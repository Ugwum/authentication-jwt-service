using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Prospa.AuthService.API.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthTokenController : ControllerBase
    {
        private readonly ILogger<AuthTokenController> _logger;  
        public AuthTokenController(ILogger<AuthTokenController> logger)
        {
            _logger = logger;
        }
    }
}
