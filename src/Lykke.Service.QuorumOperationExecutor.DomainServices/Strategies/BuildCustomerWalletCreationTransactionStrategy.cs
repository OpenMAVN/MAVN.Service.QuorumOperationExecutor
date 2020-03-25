using System;
using Lykke.PrivateBlockchain.Definitions;
using Lykke.Service.PrivateBlockchainFacade.Client;

namespace Lykke.Service.QuorumOperationExecutor.DomainServices.Strategies
{
    public class BuildCustomerWalletCreationTransactionStrategy : BuildTransactionStrategyBase<CustomerWalletContext, RegisterCustomerFunction>, IBuildTransactionStrategy
    {
        public BuildCustomerWalletCreationTransactionStrategy(
            string contractAddress)
            : base(contractAddress)
        {
        }

        public string SupportedOperationType
            => "CustomerWalletCreation";

        protected override RegisterCustomerFunction Execute(
            string from,
            Guid operationId,
            CustomerWalletContext operationPayload)
        {
            return new RegisterCustomerFunction
            {
                CustomerAddress = operationPayload.WalletAddress,
                CustomerId = operationPayload.CustomerId,
            };
        }
    }
}
