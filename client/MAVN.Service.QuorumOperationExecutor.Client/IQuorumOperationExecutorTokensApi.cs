using System.Threading.Tasks;
using JetBrains.Annotations;
using MAVN.Service.QuorumOperationExecutor.Client.Models.Responses;
using Refit;

namespace MAVN.Service.QuorumOperationExecutor.Client
{
    /// <summary>
    /// QuorumOperationExecutor client API interface.
    /// </summary>
    [PublicAPI]
    public interface IQuorumOperationExecutorTokensApi
    {
        /// <summary>
        /// Method for getting total amount of tokens in private blockchain
        /// </summary>
        /// <returns>TotalTokensSupplyResponse which holds total tokens amount</returns>
        [Get("/api/tokens/total-supply")]
        Task<TotalTokensSupplyResponse> GetTokensTotalSupplyAsync();
    }
}
