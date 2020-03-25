using System;
using Nethereum.Signer;

namespace Lykke.Service.QuorumOperationExecutor.DomainServices.Strategies
{
    public interface IBuildTransactionStrategy
    {
        string SupportedOperationType { get; }
        
        Transaction Execute(
            string from,
            long nonce,
            Guid operationId,
            string operationPayloadJson);
    }
}
