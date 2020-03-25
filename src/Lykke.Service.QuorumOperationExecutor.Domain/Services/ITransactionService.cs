using System;
using System.Threading.Tasks;

namespace Lykke.Service.QuorumOperationExecutor.Domain.Services
{
    public interface ITransactionService
    {
        Task<Guid?> TryGetOperationIdAsync(
            string txHash);

        Task<TransactionState?> TryGetTransactionStateAsync(
            string txHash);
    }
}
