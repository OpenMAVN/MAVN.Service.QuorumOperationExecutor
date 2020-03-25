using System;
using Lykke.PrivateBlockchain.Definitions;
using Lykke.Service.PrivateBlockchainFacade.Contract.Operations;
using Nethereum.Hex.HexConvertors.Extensions;

namespace Lykke.Service.QuorumOperationExecutor.DomainServices.Strategies
{
    public class BuildTransferStrategy : BuildTransactionStrategyBase<TokensTransferContext, SendFunction>, IBuildTransactionStrategy
    {
        public BuildTransferStrategy(string contractAddress) : base(contractAddress)
        {
        }

        public string SupportedOperationType
            => "TokensTransfer";

        protected override SendFunction Execute(
            string from,
            Guid operationId,
            TokensTransferContext operationPayload)
        {
            return new SendFunction
            {
                Amount = operationPayload.Amount.ToAtto(),
                FromAddress = operationPayload.SenderWalletAddress,
                Recipient = operationPayload.RecipientWalletAddress,
                Data = !string.IsNullOrEmpty(operationPayload.AdditionalData) ? operationPayload.AdditionalData.HexToByteArray() : new byte[] { }
            };
        }
    }
}
