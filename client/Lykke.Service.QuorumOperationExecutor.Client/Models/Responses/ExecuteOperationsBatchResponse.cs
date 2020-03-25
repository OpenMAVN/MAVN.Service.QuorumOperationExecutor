using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Service.QuorumOperationExecutor.Client.Models.Responses
{
    /// <summary>
    ///    Operations batch execution response.
    /// </summary>
    [PublicAPI]
    public class ExecuteOperationsBatchResponse
    {
        /// <summary>Operation execution error.</summary>
        public ExecuteOperationError Error { get; set; }

        /// <summary>TransactionId to hash dictionary.</summary>
        public Dictionary<Guid, string> TxHashesDict { get; set; }
    }
}
