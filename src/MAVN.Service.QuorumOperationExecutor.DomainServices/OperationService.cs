using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Nethereum.Extension;
using MAVN.Service.QuorumOperationExecutor.Domain;
using MAVN.Service.QuorumOperationExecutor.Domain.Repositories;
using MAVN.Service.QuorumOperationExecutor.Domain.Services;
using MAVN.Service.QuorumOperationExecutor.DomainServices.Extensions;
using MAVN.Service.QuorumOperationExecutor.DomainServices.Strategies;
using MAVN.Service.QuorumTransactionSigner.Client;
using MoreLinq;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.Signer;
using Nethereum.Util;
using Polly;
using Polly.Retry;

namespace MAVN.Service.QuorumOperationExecutor.DomainServices
{
    public class OperationService : IOperationService
    {
        private readonly Dictionary<string, IBuildTransactionStrategy> _buildTransactionStrategies;
        private readonly IBlockchain _ethereumApi;
        private readonly ILog _log;
        private readonly IOperationRepository _operationRepository;
        private readonly ITransactionService _transactionService;
        private readonly IQuorumTransactionSignerClient _transactionSigner;
        private readonly int _maxThreadCount;
        private readonly RetryPolicy _sendTransactionRetryPolicy;

        private const int DefaultMaxThreadCount = 50;
        private const int DefaultRetryCount = 10;
        private const int WaitMilliSec = 100;
        private const int SendBatchRetryIterationsCounter = 51;

        public OperationService(
            IEnumerable<IBuildTransactionStrategy> buildTransactionStrategies,
            IBlockchain ethereumApi,
            ILogFactory logFactory,
            IOperationRepository operationRepository,
            ITransactionService transactionService,
            IQuorumTransactionSignerClient transactionSigner,
            int? maxThreadCount = null)
        {
            _buildTransactionStrategies =
                buildTransactionStrategies.ToDictionary(x => x.SupportedOperationType, x => x);
            _ethereumApi = ethereumApi;
            _log = logFactory.CreateLog(this);
            _operationRepository = operationRepository;
            _transactionService = transactionService;
            _transactionSigner = transactionSigner;
            _maxThreadCount = maxThreadCount ?? DefaultMaxThreadCount;

            _sendTransactionRetryPolicy = Policy
                .Handle<RpcClientTimeoutException>()
                .WaitAndRetryAsync(
                    DefaultRetryCount,
                    attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    (ex, _) => _log.Error("Sending transaction with retry", ex));
        }

        public async Task<string> ExecuteOperationAsync(
            Guid operationId,
            string operationType,
            string masterWalletAddress,
            long nonce,
            string payloadJson)
        {
            using (_log.BeginScope($"{nameof(ExecuteOperationAsync)}-{Guid.NewGuid()}"))
            {
                var (transactionData, transactionHash) =
                    await _operationRepository.GetOrCreateOperationAsync(operationId);

                if (transactionData == null)
                {
                    var txDetails = await BuildTransactionAsync(operationId, operationType, masterWalletAddress, nonce,
                        payloadJson);

                    // Transaction has already been built, but has not been broadcasted yet
                    await SendTransactionAsync(
                        operationId,
                        txDetails.Data,
                        txDetails.Hash);

                    return txDetails.Hash;
                }

                // Transaction has already been built and broadcasted

                #region Logging

                if (transactionData == null)
                    _log.Warning("Operation has hash, but has no data.", context: new {operationId, operationType});

                _log.Info("Operation has already been built and broadcasted.",
                    new {operationId, operationType, transactionHash});

                #endregion

                return transactionHash;
            }
        }

        public async Task<Dictionary<Guid, string>> ExecuteOperationsBatchAsync(
            string masterWalletAddress,
            IEnumerable<OperationDetails> operationsData)
        {
            using (_log.BeginScope($"{nameof(ExecuteOperationAsync)}-{Guid.NewGuid()}"))
            {
                var result = new Dictionary<Guid, string>();

                var existingOperationsDict =
                    await _operationRepository.GetOrCreateOperationsAsync(operationsData.Select(i => i.Id));

                var notBuiltOperations = new List<OperationDetails>();
                var notBroadcastedOperations = new Dictionary<Guid, (string, string)>();

                foreach (var operationData in operationsData)
                {
                    if (existingOperationsDict.ContainsKey(operationData.Id))
                    {
                        var operationDbData = existingOperationsDict[operationData.Id];
                        if (operationDbData.TxData == null) //empty data, hash also should be empty
                        {
                            notBuiltOperations.Add(operationData);
                        }
                        else
                        {
                            notBroadcastedOperations.Add(operationData.Id, operationDbData);
                            result.Add(operationData.Id, operationDbData.TxHash);
                        }
                    }
                    else
                    {
                        notBuiltOperations.Add(operationData);
                    }
                }

                if (notBuiltOperations.Any())
                {
                    var txDataDict = await BuildTransactionsAsync(masterWalletAddress, notBuiltOperations);
                    foreach (var pair in txDataDict)
                    {
                        notBroadcastedOperations.Add(pair.Key, pair.Value);
                        result.Add(pair.Key, pair.Value.TxHash);
                    }
                }

                if (notBroadcastedOperations.Any())
                    await SendTransactionsAsync(notBroadcastedOperations);

                return result;
            }
        }

        public bool IsOperationTypeSupported(string operationType)
        {
            return _buildTransactionStrategies.ContainsKey(operationType);
        }

        public bool AreOperationTypesSupported(IEnumerable<string> operationTypes)
        {
            return operationTypes.ToHashSet().All(o => _buildTransactionStrategies.ContainsKey(o));
        }

        public async Task<(OperationState? OperationState, string TransactionHash)> TryGetOperationStateAsync(
            Guid operationId)
        {
            var (transactionData, transactionHash) = await _operationRepository.TryGetOperationAsync(operationId);

            if (transactionHash != null)
            {
                var transactionState = await _transactionService.TryGetTransactionStateAsync(transactionHash);

                OperationState operationState;

                switch (transactionState)
                {
                    case TransactionState.Pending:
                        operationState = OperationState.Pending;
                        break;
                    case TransactionState.Succeeded:
                        operationState = OperationState.Succeeded;
                        break;
                    case TransactionState.Failed:
                        operationState = OperationState.Failed;
                        break;
                    case null:
                        operationState = OperationState.Built;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            $"Transaction state [{transactionState}] is not supported.");
                }

                return (operationState, transactionHash);
            }

            return (transactionData != null ? OperationState.Built : (OperationState?) null, null);
        }

        private async Task<(string Data, string Hash)> BuildTransactionAsync(
            Guid operationId,
            string operationType,
            string masterWalletAddress,
            long nonce,
            string payloadJson)
        {
            Transaction transaction;

            try
            {
                transaction = _buildTransactionStrategies[operationType].Execute
                (
                    from: masterWalletAddress,
                    nonce: nonce,
                    operationId: operationId,
                    operationPayloadJson: payloadJson
                );

                #region Logging

                _log.Info
                (
                    "Transaction has been built.",
                    new {operationId, operationType, masterWalletAddress, nonce}
                );

                #endregion
            }
            catch (Exception e)
            {
                #region Logging

                _log.Error
                (
                    e,
                    "Failed to build transaction.",
                    new {operationId, operationType, masterWalletAddress, nonce}
                );

                #endregion

                throw;
            }

            try
            {
                var (v, r, s) = await _transactionSigner.SignTransactionAsync(masterWalletAddress, transaction.RawHash);

                var signature = EthECDSASignatureFactory.FromComponents(r: r, s: s, v: v);

                transaction.SetSignature(signature);

                #region Logging

                _log.Info
                (
                    "Transaction has been signed.",
                    new {operationId, operationType, masterWalletAddress, nonce}
                );

                #endregion
            }
            catch (Exception e)
            {
                #region Logging

                _log.Error
                (
                    e,
                    "Failed to sign transaction.",
                    new {operationId, operationType, masterWalletAddress, nonce}
                );

                #endregion

                throw;
            }

            var transactionData = transaction.GetRLPEncoded().ToHex(true);
            var transactionHash = $"0x{Sha3Keccack.Current.CalculateHashFromHex(transactionData)}";

            try
            {
                await _operationRepository.SetTransactionDataAndHashAsync(
                    operationId,
                    transactionData,
                    transactionHash);

                #region Logging

                _log.Info
                (
                    "Transaction data has been saved.",
                    new {operationId, operationType, masterWalletAddress, nonce}
                );

                #endregion
            }
            catch (Exception e)
            {
                #region Logging

                _log.Error
                (
                    e,
                    "Failed to save transaction data.",
                    new {operationId, operationType, masterWalletAddress, nonce}
                );

                #endregion

                throw;
            }

            return (transactionData, transactionHash);
        }

        private async Task<Dictionary<Guid, (string TxData, string TxHash)>> BuildTransactionsAsync(
            string masterWalletAddress, List<OperationDetails> operations)
        {
            var txDict = new Dictionary<Guid, Transaction>();

            foreach (var operation in operations)
            {
                try
                {
                    var transaction = _buildTransactionStrategies[operation.Type].Execute
                    (
                        from: masterWalletAddress,
                        nonce: operation.Nonce,
                        operationId: operation.Id,
                        operationPayloadJson: operation.JsonPayload
                    );
                    txDict.Add(operation.Id, transaction);
                }
                catch (Exception e)
                {
                    _log.Error
                    (
                        e,
                        "Failed to build transaction.",
                        new {operation.Id, operation.Type, masterWalletAddress, operation.Nonce}
                    );

                    throw;
                }
            }

            _log.Info($"{txDict.Count} transactions have been built.");

            try
            {
                var keys = txDict.Keys.ToArray();
                var signedData = await _transactionSigner.SignTransactionsAsync(
                    masterWalletAddress, keys.Select(k => txDict[k].RawHash).ToList());

                for (int i = 0; i < keys.Length; ++i)
                {
                    var (v, r, s) = signedData[i];
                    var signature = EthECDSASignatureFactory.FromComponents(r, s, v);
                    txDict[keys[i]].SetSignature(signature);
                }

                _log.Info($"{txDict.Count} transactions have been signed.");
            }
            catch (Exception e)
            {
                _log.Error(e, $"Failed to sign {txDict.Count} transactions.");

                throw;
            }

            var transactionsDataAndHashes = new Dictionary<Guid, (string, string)>(txDict.Count);
            foreach (var pair in txDict)
            {
                var txData = pair.Value.GetRLPEncoded().ToHex(true);
                var txHash = $"0x{Sha3Keccack.Current.CalculateHashFromHex(txData)}";
                transactionsDataAndHashes.Add(pair.Key, (txData, txHash));
            }

            try
            {
                await _operationRepository.SetTransactionsDataAndHashesAsync(transactionsDataAndHashes);

                _log.Info($"Data for {transactionsDataAndHashes.Count} transactions have been saved.");
            }
            catch (Exception e)
            {
                _log.Error(e, "Failed to save transactions data.");

                throw;
            }

            return transactionsDataAndHashes;
        }

        private async Task SendTransactionAsync(
            Guid operationId,
            string transactionData,
            string transactionHash)
        {
            try
            {
                await _sendTransactionRetryPolicy.ExecuteAsync(() =>
                    _ethereumApi.TransactionApi().SendTransactionAsync(transactionData, transactionHash));

                _log.Info("Transaction has been broadcasted.", new {operationId, transactionHash});
            }
            catch (Exception e)
            {
                _log.Error(e, "Failed to broadcast transaction for operation.", new {operationId});

                throw;
            }
        }

        private async Task SendTransactionsAsync(
            Dictionary<Guid, (string TxData, string TxHash)> operationsDataAndHashesDict)
        {
            var batches = operationsDataAndHashesDict.Batch(_maxThreadCount);

            foreach (var batchItem in batches)
            {
                await _sendTransactionRetryPolicy.ExecuteAsync(() => SendTransactionsInBatchAsync(batchItem));
            }
        }

        private async Task SendTransactionsInBatchAsync(
            IEnumerable<KeyValuePair<Guid, (string TxData, string TxHash)>> batch)
        {
            var batchList = batch.ToList();

            var operations = batchList.Aggregate("", (s, pair) => s += $", {pair.Key}");

            var initialBatchSize = batchList.Count;

            try
            {
                _log.Info("Trying to process transaction batch.", new {initialBatchSize});

                var countIteration = 0;

                while (countIteration <= SendBatchRetryIterationsCounter)
                {
                    await FilterExistingTransactionAsync(batchList);

                    if (!batchList.Any())
                    {
                        break;
                    }

                    if (countIteration % 10 == 0)
                    {
                        var result = await SendRawTransactionsAsync(batchList);

                        FilterFailTransactionFromBatch(result, batchList);
                    }

                    if (countIteration == SendBatchRetryIterationsCounter)
                    {
                        _log.Error("Cannot send the whole batch to the node",
                            context: new {Operations = batchList.Select(i => i.Key).ToArray(), batchList.Count});

                        throw new Exception(
                            $"Cannot send the whole batch to the node. Transactions amount = {batchList.Count}");
                    }

                    countIteration++;

                    if (countIteration > 1)
                    {
                        _log.Info($"Waiting {WaitMilliSec}ms to check transactions");

                        await Task.Delay(WaitMilliSec);
                    }
                }

                _log.Info("Transactions batch has been processed.",
                    new {BatchSize = initialBatchSize, OperationList = operations});
            }
            catch (Exception e)
            {
                _log.Error(e, "Failed to broadcast batch transactions.", new {initialBatchSize});

                throw;
            }
        }

        private void FilterFailTransactionFromBatch(IEnumerable<RpcResponseMessage> result,
            ICollection<KeyValuePair<Guid, (string TxData, string TxHash)>> batch)
        {
            foreach (var message in result)
            {
                if (message.Error == null) continue;

                _log.Warning("RPC error while posting transaction",
                    context: new
                    {
                        RpcError = new
                        {
                            OperationId = message.Id.ToString(),
                            message.Error?.Code,
                            message.Error?.Message,
                            Data = message.Error?.Data.ToString()
                        }
                    });

                var item = batch.First(i => i.Key.ToString() == message.IdAsString());

                batch.Remove(item);
            }
        }

        private async Task<List<RpcResponseMessage>> SendRawTransactionsAsync(
            List<KeyValuePair<Guid, (string TxData, string TxHash)>> batch)
        {
            var request = batch.Select(i =>
                    new RpcRequestMessage(i.Key, ApiMethodNames.eth_sendRawTransaction, i.Value.TxData))
                .ToArray();
            var result = await _ethereumApi.RpcClient().ExecuteRpcBatchAsync(request);
            return result;
        }

        private async Task FilterExistingTransactionAsync(
            ICollection<KeyValuePair<Guid, (string TxData, string TxHash)>> batch)
        {
            var batchRequest = batch
                .Select(t => new RpcRequestMessage(t.Key, "eth_getTransactionByHash", t.Value.TxHash))
                .ToArray();

            var batchResult = await _ethereumApi.RpcClient().ExecuteRpcBatchAsync(batchRequest);

            var countExistingTransaction = batchResult.Count(i => i.Result?.HasValues == true);

            if (countExistingTransaction > 0)
            {
                foreach (var message in batchResult)
                {
                    if (message.Result == null) continue;

                    var item = batch.First(i => i.Key == message.IdAsGuid());

                    batch.Remove(item);
                }

                _log.Info("Filtered out existing transaction", new {countExistingTransaction, batchSize = batch.Count});
            }
        }
    }
}
