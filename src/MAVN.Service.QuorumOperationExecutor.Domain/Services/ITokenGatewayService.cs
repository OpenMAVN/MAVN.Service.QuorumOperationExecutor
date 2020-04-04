using System.Threading.Tasks;
using Falcon.Numerics;

namespace MAVN.Service.QuorumOperationExecutor.Domain.Services
{
    public interface ITokenGatewayService
    {
        Task<Money18> GetTransferToPublicNetworkFeeAsync();
    }
}
