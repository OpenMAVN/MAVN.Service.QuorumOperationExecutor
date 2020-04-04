using System;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.Services;

namespace MAVN.Service.QuorumOperationExecutor.DomainServices.Extensions
{
    internal static class EthApiTransactionsServiceExtensions
    {
        private static int GetTransactionPollPeriodInMilliseconds = 100;
        private static int PushTransactionRetryPeriodInMilliseconds = 1000;
        private static int GetRetriesBeforePushAgain => (int) Math.Ceiling((decimal) PushTransactionRetryPeriodInMilliseconds / GetTransactionPollPeriodInMilliseconds);

        public static async Task SendTransactionAsync(
            this IEthApiTransactionsService service,
            string signedTransactionData,
            string transactionHash,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var transaction = await service.GetTransactionByHash.SendRequestAsync(transactionHash);

            // If transaction is not null, the transaction has already been broadcasted
            // and it is not necessary to broadcast it again.
            if (transaction != null)
                return;

            // Broadcasting transaction
            try
            {
                await service.SendRawTransaction.SendRequestAsync(signedTransactionData);
            }
            // Unfortunately, there are neither specific error code, nor specific properties for this case in an exception  
            catch (RpcResponseException e) when (e.RpcError.Message == $"known transaction: {transactionHash}")
            {
                return;
            }

            // ...and waiting, until it appears on a blockchain node.
            transaction = await service.GetTransactionByHash.SendRequestAsync(transactionHash);

            var getTransactionCounter = 1;

            while (transaction == null)
            {
                // checking if it is time to re-push transaction
                if (getTransactionCounter == GetRetriesBeforePushAgain)
                {
                    try
                    {
                        await service.SendRawTransaction.SendRequestAsync(signedTransactionData);
                    }
                    catch (RpcResponseException e) when (e.RpcError.Message == $"known transaction: {transactionHash}")
                    {
                        return;
                    }

                    getTransactionCounter = 0;
                }

                await Task.Delay(GetTransactionPollPeriodInMilliseconds, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                transaction = await service.GetTransactionByHash.SendRequestAsync(transactionHash);

                getTransactionCounter++;
            }
        }
    }
}
