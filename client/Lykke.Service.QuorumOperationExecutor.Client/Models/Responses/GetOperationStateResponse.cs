using System;
using JetBrains.Annotations;

namespace Lykke.Service.QuorumOperationExecutor.Client.Models.Responses
{
    /// <summary>
    ///    Get operation state response.
    /// </summary>
    [PublicAPI]
    public class GetOperationStateResponse
    {
        /// <summary>
        ///    The operation execution error.
        /// </summary>
        public GetOperationStateError Error { get; set; }
        
        /// <summary>
        ///    The id of the operation.
        /// </summary>
        public Guid OperationId { get; set; }
        
        /// <summary>
        ///    The operation state.
        /// </summary>
        public OperationState OperationState { get; set; }
        
        /// <summary>
        ///    The hash of the related transaction.
        /// </summary>
        public string TransactionHash { get; set; }
    }
}
