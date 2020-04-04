using System;
using JetBrains.Annotations;

namespace MAVN.Service.QuorumOperationExecutor.Client.Models.Responses
{
    /// <summary>
    ///    Get transaction state response.
    /// </summary>
    [PublicAPI]
    public class GetTransactionStateResponse
    {
        /// <summary>
        ///    The operation execution error.
        /// </summary>
        public GetTransactionStateError Error { get; set; }
        
        /// <summary>
        ///    The id of the related operation.
        /// </summary>
        public Guid? OperationId { get; set; }
        
        /// <summary>
        ///    The has of the transaction.
        /// </summary>
        public string TransactionHash { get; set; }

        /// <summary>
        ///    The state of the transaction.
        /// </summary>
        public TransactionState TransactionState { get; set; }
    }
}
