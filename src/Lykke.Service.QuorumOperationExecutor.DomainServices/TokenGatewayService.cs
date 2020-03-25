using System.Threading.Tasks;
using Falcon.Numerics;
using Lykke.Service.QuorumOperationExecutor.Domain.Services;

namespace Lykke.Service.QuorumOperationExecutor.DomainServices
{
    public class TokenGatewayService : ITokenGatewayService
    {
        private readonly IBlockchain _blockchain;

        public TokenGatewayService(IBlockchain blockchain)
        {
            _blockchain = blockchain;
        }

        public async Task<Money18> GetTransferToPublicNetworkFeeAsync()
        {
            var result = await _blockchain.TokenGatewayService().TransferToPublicNetworkFeeQueryAsync();

            return Money18.CreateFromAtto(result);
        }
    }
}
