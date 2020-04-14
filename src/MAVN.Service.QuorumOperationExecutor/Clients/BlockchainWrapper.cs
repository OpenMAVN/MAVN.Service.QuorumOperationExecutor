using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Logs.Nethereum;
using Lykke.Nethereum.Extension;
using MAVN.Service.QuorumOperationExecutor.DomainServices;
using Nethereum.Contracts.Services;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.WebSocketClient;
using Nethereum.RPC.Eth.Services;
using Nethereum.Web3;
using MVNGatewayService = MAVN.PrivateBlockchain.Services.MVNGatewayService;
using MVNTokenService = MAVN.PrivateBlockchain.Services.MVNTokenService;

namespace MAVN.Service.QuorumOperationExecutor.Clients
{
    public class BlockchainWrapper: IBlockchain, IDisposable, IStartable
    {
        private readonly Func<Task<string[]>> _connStringListGetter;
        private readonly string _tokenContractAddress;
        private readonly string _tokenGatewayContractAddress;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ConcurrentDictionary<string, EthConnection> _connectionList = new ConcurrentDictionary<string, EthConnection>();
        private RoundRobinAccessor<EthConnection> _activeConnectionPool;
        private readonly TimerTrigger _timer;
        private readonly ILog _log;
        private readonly NethereumLog _nethereumLog;
        private readonly TimeSpan _timeout;
        
        private const int DefaultWebSocketConnectionTimeoutInSeconds = 20;

        public BlockchainWrapper(
            Func<Task<string[]>> connStringListGetter, 
            string tokenContractAddress,
            string tokenGatewayContractAddress,
            TimeSpan? webSocketConnectionTimeout, 
            IHttpClientFactory httpClientFactory,
            ILogFactory logFactory)
        {
            _timeout = webSocketConnectionTimeout ?? TimeSpan.FromSeconds(DefaultWebSocketConnectionTimeoutInSeconds);
            _log = logFactory.CreateLog(this);
            _nethereumLog = logFactory.CreateNethereumLog(this);
            _connStringListGetter = connStringListGetter;
            _tokenContractAddress = tokenContractAddress;
            _tokenGatewayContractAddress = tokenGatewayContractAddress;
            _httpClientFactory = httpClientFactory;
            UpdateConnectionList(connStringListGetter.Invoke().Result);
            SetActiveConnectionPool();
            _timer = new TimerTrigger(GetType().Name, TimeSpan.FromSeconds(300), logFactory, TimerHandler);

            // setting up timeout for websocket connections
            ClientBase.ConnectionTimeout = _timeout;
        }

        private int CountActiveConnections { get; set; }

        public IEthApiTransactionsService TransactionApi()
        {
            if (_activeConnectionPool == null)
                throw new Exception("Do not have any active connection to blockchain");


            var count = _connectionList.Count;
            var index = 0;
            while (index++ <= count)
            {
                var connect = _activeConnectionPool.GetNext();
                if (connect.IsWebSocket)
                    return connect.Transactions;
            }

            return _activeConnectionPool.GetNext().Transactions;
        }
        
        public ILykkeJsonRpcClient RpcClient()
        {
            if (_activeConnectionPool == null)
                throw new Exception("Do not have any active connection to blockchain");

            var count = _connectionList.Count;
            var index = 0;
            while (index++ <= count)
            {
                var client = _activeConnectionPool.GetNext().RpcClient;
                if (client != null)
                    return client;
            }

            throw new Exception("Do not have any active HTTPS connection to blockchain");
        }

        public MVNTokenService TokenService()
        {
            if (_activeConnectionPool == null)
                throw new Exception("Do not have any active connection to blockchain");

            var count = _connectionList.Count;
            var index = 0;
            while (index++ <= count)
            {
                var connect = _activeConnectionPool.GetNext();
                if (connect.IsWebSocket)
                    return connect.TokenService;
            }

            return _activeConnectionPool.GetNext().TokenService;
        }

        public MVNGatewayService TokenGatewayService()
        {
            if (_activeConnectionPool == null)
                throw new Exception("Do not have any active connection to blockchain");

            var count = _connectionList.Count;
            var index = 0;
            while (index++ <= count)
            {
                var connect = _activeConnectionPool.GetNext();
                if (connect.IsWebSocket)
                    return connect.TokenGatewayService;
            }

            return _activeConnectionPool.GetNext().TokenGatewayService;
        }

        private EthConnection InternalCreateInstanceApi(string connString, int index)
        {
            Web3 web3;
            if (connString.StartsWith("ws"))
            {
                var wsClient = new WebSocketClient(connString, log: _nethereumLog);
                web3 = new Web3(wsClient);

                return new EthConnection(connString, 
                    wsClient, 
                    web3,
                    new MVNTokenService(web3, _tokenContractAddress),
                    new MVNGatewayService(web3, _tokenGatewayContractAddress), 
                    index, 
                    _httpClientFactory, 
                    _timeout);
            }

            web3 = new Web3(connString, _nethereumLog);
            
            return new EthConnection(connString, 
                null, 
                web3, 
                new MVNTokenService(web3, _tokenContractAddress),
                new MVNGatewayService(web3, _tokenGatewayContractAddress), 
                index, 
                _httpClientFactory, 
                _timeout);
        }

        private bool UpdateConnectionList(string[] connStringList)
        {
            var isChanged = false;

            for (var index = 0; index < connStringList.Length; index++)
            {
                if (!_connectionList.TryGetValue(connStringList[index], out var connection))
                {
                    connection = InternalCreateInstanceApi(connStringList[index], index);
                    
                    if (connection == null)
                        continue;

                    _connectionList[connection.ConnectionString] = connection;
                    isChanged = true;
                }
            }

            foreach (var connectionString in _connectionList.Keys.ToArray())
            {
                if (!connStringList.Contains(connectionString))
                {
                    _connectionList.TryRemove(connectionString, out _);
                    isChanged = true;
                }

            }

            return isChanged;
        }

        private void SetActiveConnectionPool()
        {
            var pool = _connectionList.Values.Where(c => c.IsActive).ToArray();
            if (!pool.Any())
            {
                var exception = new Exception("Do not found any active connection to blockchain");
                _log.Error(exception, "Do not found any active connection to blockchain");
                throw exception;
            }

            _activeConnectionPool = new RoundRobinAccessor<EthConnection>(pool);

            CountActiveConnections = pool.Length;
            _log.Info($"Set new active block-chain connection pool. Capacity: {CountActiveConnections}");
        }

        private async Task TimerHandler(ITimerTrigger timer, TimerTriggeredHandlerArgs args, CancellationToken cancellationtoken)
        {
            await DoUpdate();
        }

        private async Task DoUpdate()
        {
            var connStrings = await _connStringListGetter();
            var connectionListUpdated = UpdateConnectionList(connStrings);
            var connectionStatusUpdated = await UpdateActiveConnectionList();

            if (connectionListUpdated || connectionStatusUpdated)
            {
                SetActiveConnectionPool();
            }
        }

        private async Task<bool> UpdateActiveConnectionList()
        {
            var statusChanged = false;

            foreach (var connection in _connectionList.Values)
            {
                var isActive = await CheckConnection(connection);

                if (isActive == connection.IsActive)
                    continue;

                statusChanged = true;
                connection.IsActive = isActive;
            }

            return statusChanged;
        }

        private async Task<bool> CheckConnection(EthConnection connection)
        {
            try
            {
                var getBestExistingBlockNumber = await connection.EthApi.Blocks.GetBlockNumber.SendRequestAsync();
                return getBestExistingBlockNumber.Value > 0;
            }
            catch (Exception ex)
            {
                _log.Warning($"Detect death connection to BlockchainNode. Setting index: {connection.Index}", exception: ex);
                return false;
            }
        }


        private class EthConnection
        {
            public EthConnection(string connectionString, 
                WebSocketClient wsClient, 
                Web3 ethWeb3,
                MVNTokenService tokenService, 
                MVNGatewayService tokenGatewayService, 
                int index, 
                IHttpClientFactory httpClientFactory, 
                TimeSpan timeout)
            {
                ConnectionString = connectionString;
                WsClient = wsClient;
                EthWeb3 = ethWeb3;
                Index = index;
                TokenService = tokenService;
                TokenGatewayService = tokenGatewayService;
                IsWebSocket = connectionString.ToLower().StartsWith("ws");
                if (!IsWebSocket)
                    RpcClient = new LykkeJsonRpcClient(connectionString, httpClientFactory, timeout);
            }

            public bool IsActive { get; set; } = true;

            public string ConnectionString { get; }
            public WebSocketClient WsClient { get; }
            public Web3 EthWeb3 { get; }
            public int Index { get; }

            public MVNTokenService TokenService { get; }

            public MVNGatewayService TokenGatewayService { get; }

            public ILykkeJsonRpcClient RpcClient { get; }

            public bool IsWebSocket { get; }

            public IEthApiContractService EthApi => EthWeb3.Eth;

            public IEthApiTransactionsService Transactions => EthWeb3.Eth.Transactions;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void Start()
        {
            _timer.Start();
        }
    }
}
