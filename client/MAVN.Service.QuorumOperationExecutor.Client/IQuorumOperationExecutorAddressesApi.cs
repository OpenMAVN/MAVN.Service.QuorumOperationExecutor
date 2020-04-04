using System.Threading.Tasks;
using JetBrains.Annotations;
using MAVN.Service.QuorumOperationExecutor.Client.Models.Responses;
using Refit;

namespace MAVN.Service.QuorumOperationExecutor.Client
{
    /// <summary>
    ///    QuorumOperationExecutor client API interface.
    /// </summary>
    [PublicAPI]
    public interface IQuorumOperationExecutorAddressesApi
    {
        /// <summary>
        /// Returns the balance of a specific address
        /// </summary>
        /// <param name="address"></param>
        [Get("/api/addresses/{address}/balance")]
        Task<AddressBalanceResponse> GetBalanceForAddressAsync(string address);
    }
}
