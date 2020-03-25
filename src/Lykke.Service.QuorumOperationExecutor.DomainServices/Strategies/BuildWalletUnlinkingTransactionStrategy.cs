using System;
using Lykke.PrivateBlockchain.Definitions;
using Lykke.Service.PrivateBlockchainFacade.Contract.Operations;

namespace Lykke.Service.QuorumOperationExecutor.DomainServices.Strategies
{
    public class BuildWalletUnlinkingTransactionStrategy : BuildTransactionStrategyBase<WalletUnlinkingContext, UnlinkPublicAccountFunction>,  IBuildTransactionStrategy
    {
        public BuildWalletUnlinkingTransactionStrategy(string contractAddress) : base(contractAddress)
        {
        }

        public string SupportedOperationType
            => "WalletUnlinking";

        protected override UnlinkPublicAccountFunction Execute(string @from, Guid operationId, WalletUnlinkingContext operationPayload)
        {
            return new UnlinkPublicAccountFunction
            {
                InternalAccount = operationPayload.InternalWalletAddress
            };
        }
    }
}
