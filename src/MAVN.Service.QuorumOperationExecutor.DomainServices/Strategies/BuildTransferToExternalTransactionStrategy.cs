using System;
using MAVN.PrivateBlockchain.Definitions;
using MAVN.Service.PrivateBlockchainFacade.Contract.Operations;
using Nethereum.Hex.HexConvertors.Extensions;

namespace MAVN.Service.QuorumOperationExecutor.DomainServices.Strategies
{
    public class BuildTransferToExternalTransactionStrategy : BuildTransactionStrategyBase<TransferToExternalContext, SendFunction>, IBuildTransactionStrategy
    {
        public BuildTransferToExternalTransactionStrategy(string contractAddress) : base(contractAddress)
        {
        }

        public string SupportedOperationType
            => "TransferToExternal";

        protected override SendFunction Execute(
            string from,
            Guid operationId,
            TransferToExternalContext operationPayload)
        {
            return new SendFunction
            {
                Amount = operationPayload.Amount.ToAtto(),
                FromAddress = operationPayload.PrivateWalletAddress,
                Recipient = operationPayload.RecipientContractAddress,
                Data = new byte[] { }
            };
        }
    }
}
