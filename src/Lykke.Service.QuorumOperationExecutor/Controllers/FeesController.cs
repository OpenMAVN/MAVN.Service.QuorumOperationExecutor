using System.Threading.Tasks;
using Lykke.Service.QuorumOperationExecutor.Client;
using Lykke.Service.QuorumOperationExecutor.Client.Models.Responses;
using Lykke.Service.QuorumOperationExecutor.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.QuorumOperationExecutor.Controllers
{

    [ApiController]
    [Route("api/fees")]
    public class FeesController : ControllerBase, IQuorumOperationExecutorFeesApi
    {
        private readonly ITokenGatewayService _tokenGatewayService;

        public FeesController(ITokenGatewayService tokenGatewayService)
        {
            _tokenGatewayService = tokenGatewayService;
        }

        [HttpGet("transfer-to-public")]
        public async Task<TransfersToPublicFeeResponse> GetTransferToPublicFeeAsync()
        {
            return new TransfersToPublicFeeResponse
            {
                Fee = await _tokenGatewayService.GetTransferToPublicNetworkFeeAsync()
            };
        }
    }
}
