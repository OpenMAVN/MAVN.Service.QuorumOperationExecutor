using System.Threading.Tasks;
using Lykke.Service.QuorumOperationExecutor.Client;
using Lykke.Service.QuorumOperationExecutor.Client.Models.Responses;
using Lykke.Service.QuorumOperationExecutor.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.QuorumOperationExecutor.Controllers
{
    [ApiController]
    [Route("api/tokens")]
    public class TokensController : ControllerBase, IQuorumOperationExecutorTokensApi
    {
        private readonly ITokenService _tokenService;
        
        public TokensController(
            ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpGet("total-supply")]
        public async Task<TotalTokensSupplyResponse> GetTokensTotalSupplyAsync()
        {
            return new TotalTokensSupplyResponse
            {
                TotalTokensAmount = await _tokenService.GetTotalSupplyAsync()
            };
        }
    }
}
