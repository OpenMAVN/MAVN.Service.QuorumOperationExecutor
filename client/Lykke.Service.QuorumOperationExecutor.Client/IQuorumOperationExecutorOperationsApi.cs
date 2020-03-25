using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.QuorumOperationExecutor.Client.Models.Requests;
using Lykke.Service.QuorumOperationExecutor.Client.Models.Responses;
using Refit;

namespace Lykke.Service.QuorumOperationExecutor.Client
{
    /// <summary>
    ///    QuorumOperationExecutor client API interface.
    /// </summary>
    [PublicAPI]
    public interface IQuorumOperationExecutorOperationsApi
    {
        /// <summary>
        ///    Execute an operation.
        /// </summary>
        /// <returns>
        ///    Operation execution response, which contains hash of a transaction related to the operation.
        /// </returns>
        [Post("/api/operations/{operationId}")]
        Task<ExecuteOperationResponse> ExecuteOperationAsync(
            Guid operationId,
            [Body] ExecuteOperationRequest request);

        /// <summary>
        ///    Execute operations batch.
        /// </summary>
        /// <returns>
        ///    Operations execution response, which contains hashes of related transactions.
        /// </returns>
        [Post("/api/operations")]
        Task<ExecuteOperationsBatchResponse> ExecuteOperationsBatchAsync([Body] ExecuteOperationsBatchRequest request);

        /// <summary>
        ///    Get an operation state.
        /// </summary>
        /// <returns>
        ///    Operation state response, which contains hash of a transaction related to the operation.
        /// </returns>
        [Get("/api/operations/{operationId}/state")]
        Task<GetOperationStateResponse> GetOperationStateAsync(Guid operationId);
    }
}
