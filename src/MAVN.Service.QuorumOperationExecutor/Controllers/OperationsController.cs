using System;
using System.Linq;
using System.Threading.Tasks;
using MAVN.Service.QuorumOperationExecutor.Client;
using MAVN.Service.QuorumOperationExecutor.Client.Models.Requests;
using MAVN.Service.QuorumOperationExecutor.Client.Models.Responses;
using MAVN.Service.QuorumOperationExecutor.Domain;
using MAVN.Service.QuorumOperationExecutor.Domain.Services;
using Lykke.Service.QuorumTransactionSigner.Client.Exceptions;
using Microsoft.AspNetCore.Mvc;

using ClientOperationState = MAVN.Service.QuorumOperationExecutor.Client.Models.Responses.OperationState;
using DomainOperationState = MAVN.Service.QuorumOperationExecutor.Domain.OperationState;

namespace MAVN.Service.QuorumOperationExecutor.Controllers
{
    [ApiController]
    [Route("api/operations")]
    public class OperationsController : ControllerBase, IQuorumOperationExecutorOperationsApi
    {
        private readonly IOperationService _operationService;

        public OperationsController(
            IOperationService operationService)
        {
            _operationService = operationService;
        }

        [HttpPost("{operationId}")]
        public async Task<ExecuteOperationResponse> ExecuteOperationAsync(
            [FromRoute] Guid operationId,
            [FromBody] ExecuteOperationRequest request)
        {
            var response = new ExecuteOperationResponse();

            if (_operationService.IsOperationTypeSupported(request.Type))
            {
                try
                {
                    response.TxHash = await _operationService.ExecuteOperationAsync
                    (
                        operationId,
                        request.Type,
                        request.MasterWalletAddress,
                        request.Nonce,
                        request.PayloadJson
                    );
                }
                catch (WalletNotFoundException)
                {
                    response.Error = ExecuteOperationError.MasterWalletNotFound;
                }
            }
            else
            {
                response.Error = ExecuteOperationError.NotSupportedOperationType;
            }

            return response;
        }

        /// <summary>
        ///    Execute operations batch.
        /// </summary>
        /// <returns>
        ///    Operations execution response, which contains hashes of related transactions.
        /// </returns>
        [HttpPost]
        public async Task<ExecuteOperationsBatchResponse> ExecuteOperationsBatchAsync([FromBody] ExecuteOperationsBatchRequest request)
        {
            var response = new ExecuteOperationsBatchResponse();

            if (_operationService.AreOperationTypesSupported(request.Operations.Select(o => o.Type)))
            {
                try
                {
                    response.TxHashesDict = await _operationService.ExecuteOperationsBatchAsync
                    (
                        request.MasterWalletAddress,
                        request.Operations.Select(i =>
                            new OperationDetails
                            {
                                Id = i.OperationId,
                                Type = i.Type,
                                Nonce = i.Nonce,
                                JsonPayload = i.PayloadJson
                            })
                    );
                }
                catch (WalletNotFoundException)
                {
                    response.Error = ExecuteOperationError.MasterWalletNotFound;
                }
            }
            else
            {
                response.Error = ExecuteOperationError.NotSupportedOperationType;
            }

            return response;
        }

        [HttpGet("{operationId}/state")]
        public async Task<GetOperationStateResponse> GetOperationStateAsync(
            Guid operationId)
        {
            var (operationState, transactionHash) = await _operationService.TryGetOperationStateAsync(operationId);
            
            var response = new GetOperationStateResponse
            {
                OperationId = operationId,
                TransactionHash = transactionHash
            };

            if (operationState != null)
            {
                switch (operationState)
                {
                    case DomainOperationState.Built:
                        response.OperationState = ClientOperationState.Built;
                        break;
                    case DomainOperationState.Pending:
                        response.OperationState = ClientOperationState.Pending;
                        break;
                    case DomainOperationState.Succeeded:
                        response.OperationState = ClientOperationState.Succeeded;
                        break;
                    case DomainOperationState.Failed:
                        response.OperationState = ClientOperationState.Failed;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Operation state [{operationState}] is not supported.");
                }
            }
            else
            {
                response.Error = GetOperationStateError.OperationNotFound;
            }

            return response;
        }
    }
}
