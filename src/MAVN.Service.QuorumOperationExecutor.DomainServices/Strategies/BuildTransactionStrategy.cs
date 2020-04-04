using System;
using Lykke.Service.PrivateBlockchainFacade.Contract.Operations;
using Nethereum.Signer;
using Newtonsoft.Json;

namespace MAVN.Service.QuorumOperationExecutor.DomainServices.Strategies
{
    public class BuildTransactionStrategy : IBuildTransactionStrategy
    {
        public string SupportedOperationType
            => "GenericOperation";
        
        public Transaction Execute(
            string from,
            long nonce,
            Guid operationId,
            string operationPayloadJson)
        {
            var operationPayload = JsonConvert.DeserializeObject<GenericOperationContext>(operationPayloadJson);
            
            return new Transaction(
                to: operationPayload.TargetWalletAddress,
                amount: 0,
                nonce: nonce,
                gasPrice: 0,
                gasLimit: 1000000,
                data: operationPayload.Data
            );
        }
    }
}
