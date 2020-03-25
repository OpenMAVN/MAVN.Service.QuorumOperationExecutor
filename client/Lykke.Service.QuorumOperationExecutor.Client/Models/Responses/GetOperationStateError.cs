using JetBrains.Annotations;

namespace Lykke.Service.QuorumOperationExecutor.Client.Models.Responses
{
    /// <summary>
    ///    Get operation state error enum.
    /// </summary>
    [PublicAPI]
    public enum GetOperationStateError
    {
        /// <summary>
        ///    Operation state retrieved with no errors.
        /// </summary>
        None,
        
        /// <summary>
        ///    Operation has not been found.
        /// </summary>
        OperationNotFound
    }
}
