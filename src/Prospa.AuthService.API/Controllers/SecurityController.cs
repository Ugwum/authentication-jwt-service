using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Prospa.AuthService.API.Controllers
{
    [Route("api/v1/security")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly ILogger<SecurityController> _logger;
        public SecurityController(ILogger<SecurityController> logger)
        {
            _logger = logger;
        }
    }
}
