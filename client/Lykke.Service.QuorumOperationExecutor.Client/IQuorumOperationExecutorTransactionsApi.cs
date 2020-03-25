using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.QuorumOperationExecutor.Client.Models.Responses;
using Refit;

namespace Lykke.Service.QuorumOperationExecutor.Client
{
    /// <summary>
    ///    QuorumOperationExecutor client API interface.
    /// </summary>
    [PublicAPI]
    public interface IQuorumOperationExecutorTransactionsApi
    {
        /// <summary>
        ///    Returns the state of a specified transaction.
        /// </summary>
        /// <param name="txHash">
        ///    A hash of a transaction.
        /// </param>
        [Get("/api/transactions/{txHash}/state")]
        Task<GetTransactionStateResponse> GetTransactionStateAsync(
            string txHash);
    }
}
