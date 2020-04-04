using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MAVN.Service.QuorumOperationExecutor.Client.Models.Responses;
using MoreLinq;

namespace MAVN.Service.QuorumOperationExecutor.Client
{
    /// <summary>
    /// Extension for Quorum operation executor client.
    /// </summary>
    [PublicAPI]
    public static class QuorumOperationExecutorClientExtensions
    {
        /// <summary>
        ///    Get states of specified operations.
        /// </summary>
        public static async Task<IEnumerable<GetOperationStateResponse>> GetOperationStatesAsync(
            this IQuorumOperationExecutorClient client,
            IEnumerable<Guid> operationIds,
            int batchSize = 8)
        {
            var result = new List<GetOperationStateResponse>();
            var batches = operationIds.Batch(batchSize);

            foreach (var batch in batches)
            {
                var getStates = batch
                    .Select(x => client.OperationsApi.GetOperationStateAsync(x))
                    .ToList();

                await Task.WhenAll(getStates);

                result.AddRange(getStates.Select(x => x.Result));
            }

            return result;
        }

        /// <summary>
        ///    Get states of specified transactions.
        /// </summary>
        public static async Task<IEnumerable<GetTransactionStateResponse>> GetTransactionStatesAsync(
            this IQuorumOperationExecutorClient client,
            IEnumerable<string> txHashes,
            int batchSize = 8)
        {
            var result = new List<GetTransactionStateResponse>();
            var batches = txHashes.Batch(batchSize);

            foreach (var batch in batches)
            {
                var getStates = batch
                    .Select(x => client.TransactionsApi.GetTransactionStateAsync(x))
                    .ToList();

                await Task.WhenAll(getStates);

                result.AddRange(getStates.Select(x => x.Result));
            }

            return result;
        }
    }
}
