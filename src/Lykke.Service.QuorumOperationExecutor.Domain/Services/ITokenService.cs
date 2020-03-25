using System.Threading.Tasks;
using Falcon.Numerics;

namespace Lykke.Service.QuorumOperationExecutor.Domain.Services
{
    public interface ITokenService
    {
        Task<Money18> GetBalanceAsync(string address);

        Task<Money18> GetStakedBalanceAsync(string address);

        Task<Money18> GetTotalSupplyAsync();
    }
}
