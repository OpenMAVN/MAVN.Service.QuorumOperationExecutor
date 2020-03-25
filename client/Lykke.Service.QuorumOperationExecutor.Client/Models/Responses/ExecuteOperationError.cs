using JetBrains.Annotations;

namespace Lykke.Service.QuorumOperationExecutor.Client.Models.Responses
{
    /// <summary>
    ///    Execute operation error enum.
    /// </summary>
    [PublicAPI]
    public enum ExecuteOperationError
    {
        /// <summary>
        ///    Operation has been broadcasted to the blockchain without errors.
        /// </summary>
        None,
        /// <summary>
        ///    Operation of a specified type is not supported.
        /// </summary>
        NotSupportedOperationType,
        /// <summary>
        ///    Master wallet has not been found by a Quorum Transaction Signer.
        /// </summary>
        MasterWalletNotFound,
    }
}
