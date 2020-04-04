using JetBrains.Annotations;

namespace MAVN.Service.QuorumOperationExecutor.Client
{
    /// <summary>
    ///    QuorumOperationExecutor client interface.
    /// </summary>
    [PublicAPI]
    public interface IQuorumOperationExecutorClient
    {
        /// <summary>
        ///    OperationsApi API interface.
        /// </summary>
        IQuorumOperationExecutorOperationsApi OperationsApi { get; }

        /// <summary>
        ///    TokensApi API interface.
        /// </summary>
        IQuorumOperationExecutorTokensApi TokensApi { get; }

        /// <summary>
        ///    TransactionsApi API interface.
        /// </summary>
        IQuorumOperationExecutorTransactionsApi TransactionsApi { get; }
        
        /// <summary>
        ///    AddressesApi API interface.
        /// </summary>
        IQuorumOperationExecutorAddressesApi AddressesApi { get; }

        /// <summary>
        ///    FeesApi API interface.
        /// </summary>
        IQuorumOperationExecutorFeesApi FeesApi { get; }
    }
}
