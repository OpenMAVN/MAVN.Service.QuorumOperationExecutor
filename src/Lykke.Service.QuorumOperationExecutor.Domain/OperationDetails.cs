using System;

namespace Lykke.Service.QuorumOperationExecutor.Domain
{
    public class OperationDetails
    {
        public Guid Id { get; set; }

        public string Type { get; set; }

        public long Nonce { get; set; }

        public string JsonPayload { get; set; }
    }
}
