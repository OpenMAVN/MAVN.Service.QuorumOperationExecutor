using System.Threading.Tasks;
using MAVN.Numerics;
using MAVN.Service.QuorumOperationExecutor.Domain.Services;

namespace MAVN.Service.QuorumOperationExecutor.DomainServices
{
    public class TokenService : ITokenService
    {
        private readonly IBlockchain _blockchain;

        public TokenService(IBlockchain blockchain)
        {
            _blockchain = blockchain;
        }

        public async Task<Money18> GetBalanceAsync(string address)
        {
            var result = await _blockchain.TokenService().BalanceOfQueryAsync(address);

            return Money18.CreateFromAtto(result);
        }

        public async Task<Money18> GetStakedBalanceAsync(string address)
        {
            var result = await _blockchain.TokenService().StakeOfQueryAsync(address);

            return Money18.CreateFromAtto(result);
        }

        public async Task<Money18> GetTotalSupplyAsync()
        {
            var result = await _blockchain.TokenService().TotalSupplyQueryAsync();

            return Money18.CreateFromAtto(result);
        }
    }
}
