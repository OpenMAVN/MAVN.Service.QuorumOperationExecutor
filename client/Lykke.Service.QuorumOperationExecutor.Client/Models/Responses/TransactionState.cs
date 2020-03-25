namespace Lykke.Service.QuorumOperationExecutor.Client.Models.Responses
{
    /// <summary>
    ///    Transaction state enum.
    /// </summary>
    public enum TransactionState
    {
        /// <summary>
        ///    Transaction sent to the blockchain, but is not processed yet.
        /// </summary>
        Pending,
        
        /// <summary>
        ///    Transaction executed successfully.
        /// </summary>
        Succeeded,
        
        /// <summary>
        ///    Transaction failed during execution.
        /// </summary>
        Failed
    }
}
