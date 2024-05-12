using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AuthService.Core.Filter;
using AuthService.Core.Model;
using AuthService.Core.Service;

namespace AuthService.API.Controllers
{
    //[TypeFilter(typeof(AuthClientAuthorizationFilter))]
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


        [HttpGet]
        [Route("key-exchange")]
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


        //[AllowAnonymous]
        [HttpPost]
        [Route("calculate-signature")]
        public async Task<IActionResult> CalculateSignature()
        {
            try
            {
                var clientId = Request.Headers["ClientID"].FirstOrDefault()?.Split(" ").Last().ToString();

                var result = await _securityService.CalculateSignature(clientId);
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
