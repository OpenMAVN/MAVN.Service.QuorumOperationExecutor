using System;
using System.Threading.Tasks;
using Lykke.Service.QuorumOperationExecutor.Client;
using Lykke.Service.QuorumOperationExecutor.Client.Models.Responses;
using Lykke.Service.QuorumOperationExecutor.Domain.Services;
using Microsoft.AspNetCore.Mvc;

using DomainTransactionState = Lykke.Service.QuorumOperationExecutor.Domain.TransactionState;
using ClientTransactionState = Lykke.Service.QuorumOperationExecutor.Client.Models.Responses.TransactionState;

namespace Lykke.Service.QuorumOperationExecutor.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : ControllerBase, IQuorumOperationExecutorTransactionsApi
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(
            ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("{txHash}/state")]
        public async Task<GetTransactionStateResponse> GetTransactionStateAsync(
            string txHash)
        {
            var getTransactionState = _transactionService.TryGetTransactionStateAsync(txHash);
            var getOperationId = _transactionService.TryGetOperationIdAsync(txHash);

            await Task.WhenAll(getTransactionState, getOperationId);
            
            var transactionState = getTransactionState.Result;
            var operationId = getOperationId.Result;
            
            var response = new GetTransactionStateResponse
            {
                OperationId = operationId,
                TransactionHash = txHash
            };

            if (transactionState != null)
            {
                switch (transactionState)
                {
                    case DomainTransactionState.Pending:
                        response.TransactionState = ClientTransactionState.Pending; 
                        break;
                    case DomainTransactionState.Succeeded:
                        response.TransactionState = ClientTransactionState.Succeeded;
                        break;
                    case DomainTransactionState.Failed:
                        response.TransactionState = ClientTransactionState.Failed;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Transaction state [{transactionState}] is not supported.");
                }
            }
            else
            {
                response.Error = GetTransactionStateError.TransactionNotFound;
            }

            return response;
        }
    }
}
