using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.QuorumOperationExecutor.Client.Models.Requests
{
    /// <summary>
    ///    Operations batch execution request model
    /// </summary>
    [PublicAPI]
    public class ExecuteOperationsBatchRequest
    {
        /// <summary>Address of an operation signer.</summary>
        [Required]
        public string MasterWalletAddress { get; set; }

        /// <summary>Operations data.</summary>
        [Required]
        public List<OperationData> Operations { get; set; }
    }

    /// <summary>
    /// Operation data.
    /// </summary>
    [PublicAPI]
    public class OperationData
    {
        /// <summary>Operation id.</summary>
        [Required]
        public Guid OperationId { get; set; }

        /// <summary>Type of an operation.</summary>
        [Required]
        public string Type { get; set; }

        /// <summary>Sequential operation number.</summary>
        [Required]
        public long Nonce { get; set; }

        /// <summary>Operation payload serialized into JSON.</summary>
        [Required]
        public string PayloadJson { get; set; }
    }
}
