using System;
using MAVN.PrivateBlockchain.Definitions;
using MAVN.Service.PrivateBlockchainFacade.Contract.Operations;

namespace MAVN.Service.QuorumOperationExecutor.DomainServices.Strategies
{
    public class BuildTransferToInternalTransactionStrategy : BuildTransactionStrategyBase<TransferToInternalContext, TransferFromPublicNetworkFunction>, IBuildTransactionStrategy
    {
        public BuildTransferToInternalTransactionStrategy(string contractAddress) : base(contractAddress)
        {
        }

        public string SupportedOperationType
            => "TransferToInternal";

        protected override TransferFromPublicNetworkFunction Execute(
            string from,
            Guid operationId,
            TransferToInternalContext operationPayload)
        {
            return new TransferFromPublicNetworkFunction
            {
                Amount = operationPayload.Amount.ToAtto(),
                PublicTransferId = operationPayload.PublicTransferId,
                InternalAccount = operationPayload.PrivateWalletAddress,
                PublicAccount = operationPayload.PublicWalletAddress,
            };
        }
    }
}
