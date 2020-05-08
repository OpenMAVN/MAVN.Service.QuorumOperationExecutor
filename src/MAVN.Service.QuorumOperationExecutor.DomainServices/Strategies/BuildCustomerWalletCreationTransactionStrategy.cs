using System;
using MAVN.PrivateBlockchain.Definitions;
using MAVN.Service.PrivateBlockchainFacade.Contract.Operations;

namespace MAVN.Service.QuorumOperationExecutor.DomainServices.Strategies
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
