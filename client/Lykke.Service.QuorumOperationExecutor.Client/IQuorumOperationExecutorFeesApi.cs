using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.QuorumOperationExecutor.Client.Models.Responses;
using Refit;

namespace Lykke.Service.QuorumOperationExecutor.Client
{
    /// <summary>
    /// QuorumOperationExecutor client API interface.
    /// </summary>
    [PublicAPI]
    public interface IQuorumOperationExecutorFeesApi
    {
        /// <summary>
        /// Method for getting the fee for transfer to public network
        /// </summary>
        /// <returns>TransfersToPublicFeeResponse which holds the fee amount</returns>
        [Get("/api/fees/transfer-to-public")]
        Task<TransfersToPublicFeeResponse> GetTransferToPublicFeeAsync();
    }
}
