using JetBrains.Annotations;
using MAVN.Numerics;

namespace MAVN.Service.QuorumOperationExecutor.Client.Models.Responses
{
    /// <summary>
    /// Response model which holds the balance of an address
    /// </summary>
    [PublicAPI]
    public class AddressBalanceResponse
    {
        /// <summary>
        /// Balance of an address which includes the staked amount
        /// </summary>
        public Money18 Balance { get; set; }

        /// <summary>
        /// Amount of staked tokens
        /// </summary>
        public Money18 StakedBalance { get; set; }
    }
}
