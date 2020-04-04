using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MAVN.Service.QuorumOperationExecutor.Domain.Services
{
    public interface IOperationService
    {
        Task<string> ExecuteOperationAsync(
            Guid operationId,
            string operationType,
            string masterWalletAddress,
            long nonce,
            string payloadJson);

        Task<Dictionary<Guid, string>> ExecuteOperationsBatchAsync(
            string masterWalletAddress,
            IEnumerable<OperationDetails> operationsData);

        bool IsOperationTypeSupported(string operationType);

        bool AreOperationTypesSupported(IEnumerable<string> operationTypes);

        Task<(OperationState? OperationState, string TransactionHash)> TryGetOperationStateAsync(Guid operationId);
    }
}
