using JetBrains.Annotations;

namespace MAVN.Service.QuorumOperationExecutor.Client.Models.Responses
{
    /// <summary>
    ///    Operation execution response.
    /// </summary>
    [PublicAPI]
    public class ExecuteOperationResponse
    {
        /// <summary>
        ///    Hash of a transaction related to the operation.
        /// </summary>
        public string TxHash { get; set; }
        
        /// <summary>
        ///    Operation execution error.
        /// </summary>
        public ExecuteOperationError Error { get; set; }
    }
}
