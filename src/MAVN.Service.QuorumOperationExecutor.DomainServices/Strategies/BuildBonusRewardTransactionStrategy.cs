using System;
using MAVN.PrivateBlockchain.Definitions;
using MAVN.Service.PrivateBlockchainFacade.Contract.Operations;

namespace MAVN.Service.QuorumOperationExecutor.DomainServices.Strategies
{
    public class BuildBonusRewardTransactionStrategy : BuildTransactionStrategyBase<CustomerBonusRewardContext, MintFunction>, IBuildTransactionStrategy
    {
        public BuildBonusRewardTransactionStrategy(string contractAddress) : base(contractAddress)
        {
        }

        public string SupportedOperationType
            => "CustomerBonusReward";

        protected override MintFunction Execute(
            string from,
            Guid operationId,
            CustomerBonusRewardContext operationPayload)
        {
            return new MintFunction
            {
                Account = operationPayload.WalletAddress,
                Amount = operationPayload.Amount.ToAtto()
            };
        }
    }
}
