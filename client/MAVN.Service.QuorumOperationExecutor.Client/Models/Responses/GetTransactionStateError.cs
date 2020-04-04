using JetBrains.Annotations;

namespace MAVN.Service.QuorumOperationExecutor.Client.Models.Responses
{
    /// <summary>
    ///    Get transaction state error enum.
    /// </summary>
    [PublicAPI]
    public enum GetTransactionStateError
    {
        /// <summary>
        ///    Transaction state retrieved with no errors.
        /// </summary>
        None,
        
        /// <summary>
        ///    Transaction has not been found.
        /// </summary>
        TransactionNotFound
    }
}
