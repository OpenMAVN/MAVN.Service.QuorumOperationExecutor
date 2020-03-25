using System;
using System.Threading.Tasks;
using Lykke.Service.QuorumOperationExecutor.Domain;
using Lykke.Service.QuorumOperationExecutor.Domain.Repositories;
using Lykke.Service.QuorumOperationExecutor.Domain.Services;

namespace Lykke.Service.QuorumOperationExecutor.DomainServices
{
    public class TransactionService : ITransactionService
    {
        private readonly IOperationRepository _operationRepository;
        private readonly IBlockchain _transactions;

        public TransactionService(IOperationRepository operationRepository, IBlockchain transactions)
        {
            _operationRepository = operationRepository;
            _transactions = transactions;
        }

        public Task<Guid?> TryGetOperationIdAsync(string txHash)
        {
            return _operationRepository.TryGetOperationIdAsync(txHash);
        }

        public async Task<TransactionState?> TryGetTransactionStateAsync(string txHash)
        {
            var txReceipt =  await _transactions.TransactionApi().GetTransactionReceipt.SendRequestAsync(txHash);

            if (txReceipt != null)
                return txReceipt.Status.Value == 1 ? TransactionState.Succeeded : TransactionState.Failed;

            // Pending transactions does not have a receipt
            var tx = await _transactions.TransactionApi().GetTransactionByHash.SendRequestAsync(txHash);

            return tx != null ? TransactionState.Pending : (TransactionState?)null;
        }
    }
}
