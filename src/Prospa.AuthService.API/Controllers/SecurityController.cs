using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prospa.AuthService.Core.Model;
using Prospa.AuthService.Core.Service;

namespace Prospa.AuthService.API.Controllers
{
    [Route("api/v1/security")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly ILogger<SecurityController> _logger;
        private readonly ISecurityService _securityService;
        public SecurityController(ILogger<SecurityController> logger, ISecurityService securityService)
        {
            _logger = logger;
            _securityService = securityService; 
        }


        [HttpPost]
        [Route("keyexchange")]
        public async Task<IActionResult> ExchangeKey()
        {
            try
            {
                var clientId = Request.Headers["ClientID"].FirstOrDefault()?.Split(" ").Last().ToString();
              
                var result = await _securityService.RetrievePublicKey(clientId);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, result);

                }
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred, See details {ex.Message}, {ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, new GenericErrorResponse { code = "UNEXPECTED_ERROR", message = "An unexpected error occurred" });
            }
        }
    }
}
