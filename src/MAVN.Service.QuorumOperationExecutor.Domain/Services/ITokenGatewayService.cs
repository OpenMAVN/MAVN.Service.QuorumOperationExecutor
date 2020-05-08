using System.Threading.Tasks;
using MAVN.Numerics;

namespace MAVN.Service.QuorumOperationExecutor.Domain.Services
{
    public interface ITokenGatewayService
    {
        Task<Money18> GetTransferToPublicNetworkFeeAsync();
    }
}
