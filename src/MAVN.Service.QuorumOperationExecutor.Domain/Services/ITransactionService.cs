using System;
using System.Threading.Tasks;

namespace MAVN.Service.QuorumOperationExecutor.Domain.Services
{
    public interface ITransactionService
    {
        Task<Guid?> TryGetOperationIdAsync(
            string txHash);

        Task<TransactionState?> TryGetTransactionStateAsync(
            string txHash);
    }
}
