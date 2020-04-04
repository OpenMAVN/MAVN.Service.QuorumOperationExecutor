using System;
using Nethereum.Contracts;
using Nethereum.Signer;
using Newtonsoft.Json;

namespace MAVN.Service.QuorumOperationExecutor.DomainServices.Strategies
{
    public abstract class BuildTransactionStrategyBase<TContext, TFunctionMessage>
        where TFunctionMessage : FunctionMessage
    {
        private readonly string _contractAddress;
        
        
        protected BuildTransactionStrategyBase(
            string contractAddress)
        {
            _contractAddress = contractAddress;
        }
        
        
        public Transaction Execute(
            string from,
            long nonce,
            Guid operationId,
            string operationPayloadJson)
        {
            var operationPayload = JsonConvert.DeserializeObject<TContext>(operationPayloadJson);
            var functionMessage = Execute(from, operationId, operationPayload);
            var transactionInput = functionMessage.CreateTransactionInput(_contractAddress);
            
            return new Transaction(
                to: transactionInput.To,
                amount: 0,
                nonce: nonce,
                gasPrice: 0,
                gasLimit: 1000000,
                data: transactionInput.Data
            );
        }

        protected abstract TFunctionMessage Execute(
            string from,
            Guid operationId,
            TContext operationPayload);
    }
}
