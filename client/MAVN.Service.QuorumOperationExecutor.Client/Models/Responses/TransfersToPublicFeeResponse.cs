using MAVN.Numerics;

namespace MAVN.Service.QuorumOperationExecutor.Client.Models.Responses
{
    /// <summary>
    /// response model
    /// </summary>
    public class TransfersToPublicFeeResponse
    {
        /// <summary>
        /// Value of the fee for transfers to public network
        /// </summary>
        public Money18 Fee { get; set; }
    }
}
