using System;
using Lykke.PrivateBlockchain.Definitions;
using Lykke.Service.PrivateBlockchainFacade.Contract.Operations;

namespace MAVN.Service.QuorumOperationExecutor.DomainServices.Strategies
{
    public class BuildSeizeToInternalStrategy : BuildTransactionStrategyBase<SeizeToInternalContext, SeizeFromFunction>,
        IBuildTransactionStrategy
    {
        public BuildSeizeToInternalStrategy(string contractAddress)
            : base(contractAddress)
        {
        }

        public string SupportedOperationType
            => "SeizeToInternal";

        protected override SeizeFromFunction Execute(string from, Guid operationId, SeizeToInternalContext context)
        {
            return new SeizeFromFunction
            {
                Amount = context.Amount.ToAtto(),
                Reason = context.Reason,
                Account = context.Account
            };
        }
    }
}
