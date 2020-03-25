using Lykke.Nethereum.Extension;
using Lykke.PrivateBlockchain.Services;
using Nethereum.RPC.Eth.Services;

namespace Lykke.Service.QuorumOperationExecutor.DomainServices
{
    public interface IBlockchain
    {
        IEthApiTransactionsService TransactionApi();

        MVNTokenService TokenService();

        MVNGatewayService TokenGatewayService();

        ILykkeJsonRpcClient RpcClient();
    }
}
