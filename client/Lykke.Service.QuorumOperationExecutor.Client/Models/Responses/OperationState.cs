namespace Lykke.Service.QuorumOperationExecutor.Client.Models.Responses
{
    /// <summary>
    ///    Operation state enum.
    /// </summary>
    public enum OperationState
    {
        /// <summary>
        ///    Underlying transaction has been built, but is not sent to the blockchain yet.
        /// </summary>
        Built,
        
        /// <summary>
        ///    Underlying transaction sent to the blockchain, but is not processed yet.
        /// </summary>
        Pending,
        
        /// <summary>
        ///    Underlying transaction executed successfully.
        /// </summary>
        Succeeded,
        
        /// <summary>
        ///    Underlying transaction failed during execution.
        /// </summary>
        Failed
    }
}
