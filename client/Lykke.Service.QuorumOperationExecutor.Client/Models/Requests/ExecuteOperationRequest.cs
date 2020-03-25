using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Lykke.Service.QuorumOperationExecutor.Client.Models.Requests
{
    /// <summary>
    ///    Start operation request model
    /// </summary>
    [PublicAPI]
    public class ExecuteOperationRequest
    {
        /// <summary>
        ///    Address of an operation signer.
        /// </summary>
        [Required]
        public string MasterWalletAddress { get; set; }
        
        /// <summary>
        ///    Sequential operation number.
        /// </summary>
        [Required]
        public long Nonce { get; set; }

        /// <summary>
        ///    Type of an operation.
        /// </summary>
        [Required]
        public string Type { get; set; }
        
        /// <summary>
        ///    Operation payload serialized into JSON.
        /// </summary>
        [Required]
        public string PayloadJson { get; set; }
    }
}
