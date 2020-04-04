using Falcon.Numerics;

namespace MAVN.Service.QuorumOperationExecutor.Client.Models.Responses
{
    /// <summary>
    /// Response model which holds total tokens amount.
    /// </summary>
    public class TotalTokensSupplyResponse
    {
        /// <summary>
        /// Total tokens in private blockchain
        /// </summary>
        public Money18 TotalTokensAmount { get; set; }
    }
}
