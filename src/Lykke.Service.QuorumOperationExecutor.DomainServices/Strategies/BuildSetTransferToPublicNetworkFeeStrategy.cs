using System;
using Lykke.PrivateBlockchain.Definitions;
using Lykke.Service.PrivateBlockchainFacade.Contract.Operations;

namespace Lykke.Service.QuorumOperationExecutor.DomainServices.Strategies
{
    public class BuildSetTransferToPublicNetworkFeeStrategy : BuildTransactionStrategyBase<SetTransfersToPublicFeeContext, SetTransferToPublicNetworkFeeFunction>, IBuildTransactionStrategy
    {
        public BuildSetTransferToPublicNetworkFeeStrategy(string contractAddress) : base(contractAddress)
        {
        }

        public string SupportedOperationType => "SetTransferToPublicFee";

        protected override SetTransferToPublicNetworkFeeFunction Execute(string @from, Guid operationId, SetTransfersToPublicFeeContext operationPayload)
        {
            return new SetTransferToPublicNetworkFeeFunction
            {
                Amount = operationPayload.Amount.ToAtto()
            };
        }
    }
}
