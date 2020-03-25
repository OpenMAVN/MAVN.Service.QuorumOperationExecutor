using JetBrains.Annotations;
using Lykke.HttpClientGenerator;

namespace Lykke.Service.QuorumOperationExecutor.Client
{
    /// <summary>
    /// QuorumOperationExecutor API aggregating interface.
    /// </summary>
    [PublicAPI]
    public class QuorumOperationExecutorClient : IQuorumOperationExecutorClient
    {
        /// <summary>
        ///    Client constructor.
        /// </summary>
        public QuorumOperationExecutorClient(
            IHttpClientGenerator httpClientGenerator)
        {
            OperationsApi = httpClientGenerator.Generate<IQuorumOperationExecutorOperationsApi>();
            TokensApi = httpClientGenerator.Generate<IQuorumOperationExecutorTokensApi>();
            AddressesApi = httpClientGenerator.Generate<IQuorumOperationExecutorAddressesApi>();
            TransactionsApi = httpClientGenerator.Generate<IQuorumOperationExecutorTransactionsApi>();
            FeesApi = httpClientGenerator.Generate<IQuorumOperationExecutorFeesApi>();
        }
        
        /// <inheritdoc />
        public IQuorumOperationExecutorOperationsApi OperationsApi { get; }

        /// <inheritdoc />
        public IQuorumOperationExecutorTokensApi TokensApi { get; }

        /// <inheritdoc />
        public IQuorumOperationExecutorTransactionsApi TransactionsApi { get; }
        
        /// <inheritdoc />
        public IQuorumOperationExecutorAddressesApi AddressesApi { get; }

        /// <inheritdoc />
        public IQuorumOperationExecutorFeesApi FeesApi { get; }
    }
}
