using Lykke.Nethereum.Extension;
using MAVN.PrivateBlockchain.Services;
using Nethereum.RPC.Eth.Services;

namespace MAVN.Service.QuorumOperationExecutor.DomainServices
{
    public interface IBlockchain
    {
        IEthApiTransactionsService TransactionApi();

        MVNTokenService TokenService();

        MVNGatewayService TokenGatewayService();

        ILykkeJsonRpcClient RpcClient();
    }
}
