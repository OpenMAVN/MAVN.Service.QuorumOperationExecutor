using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.QuorumOperationExecutor.Domain.Repositories
{
    public interface IOperationRepository
    {
        Task<(string Data, string Hash)> GetOrCreateOperationAsync(Guid operationId);

        Task<Dictionary<Guid, (string TxData, string TxHash)>> GetOrCreateOperationsAsync(IEnumerable<Guid> operationIds);

        Task SetTransactionDataAndHashAsync(
            Guid operationId,
            string transactionData,
            string transactionHash);

        Task SetTransactionsDataAndHashesAsync(Dictionary<Guid, (string, string)> transactionsDataAndHashesDict);

        Task<(string Data, string Hash)> TryGetOperationAsync(Guid operationId);

        Task<Guid?> TryGetOperationIdAsync(string transactionHash);
    }
}
