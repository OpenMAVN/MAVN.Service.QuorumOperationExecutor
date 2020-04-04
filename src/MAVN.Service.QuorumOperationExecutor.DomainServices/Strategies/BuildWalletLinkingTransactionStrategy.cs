using System;
using Lykke.PrivateBlockchain.Definitions;
using Lykke.Service.PrivateBlockchainFacade.Contract.Operations;

namespace MAVN.Service.QuorumOperationExecutor.DomainServices.Strategies
{
    public class BuildWalletLinkingTransactionStrategy : BuildTransactionStrategyBase<WalletLinkingContext, LinkPublicAccountFunction>,  IBuildTransactionStrategy
    {
        public BuildWalletLinkingTransactionStrategy(string contractAddress) : base(contractAddress)
        {
        }

        public string SupportedOperationType
            => "WalletLinking";

        protected override LinkPublicAccountFunction Execute(string @from, Guid operationId, WalletLinkingContext operationPayload)
        {
            return new LinkPublicAccountFunction
            {
                InternalAccount = operationPayload.InternalWalletAddress,
                PublicAccount = operationPayload.PublicWalletAddress,
                LinkingFee = operationPayload.LinkingFee.ToAtto()
            };
        }
    }
}
