using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prospa.AuthService.Core.Model.Validators;
using Prospa.AuthService.Core.Model;
using Prospa.AuthService.Core.Service.Abstractions;
using System.Net;

namespace Prospa.AuthService.API.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthTokenController : ControllerBase
    {
        private readonly ILogger<AuthTokenController> _logger;
        private readonly IUserService _userService;
        private readonly ICustomValidator _customValidator;
        public AuthTokenController(ILogger<AuthTokenController> logger, IUserService userService,
            ICustomValidator customValidator)
        {
            _logger = logger;
            _userService = userService;
            _customValidator = customValidator;
        }

        [HttpPost]
        [Route("token")]
        [ProducesResponseType(typeof(RequestResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(RequestResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(GenericErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Authenticate([FromBody] GenerateTokenRequest tokenRequest)
        {
            try
            {

                var validation = await _customValidator.ValidateAsync(tokenRequest);

                if (!validation.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, _customValidator.GetErrorResult(validation));
                }

                var result = await _userService.AuthenticateUser(tokenRequest.username, tokenRequest.password);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, result);

                }
                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred, See details {ex.Message}, {ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, new GenericErrorResponse { code = "UNEXPECTED_ERROR", message = "An unexpected error occurred" });
            }
        }

        [HttpPost]
        [Route("revoketoken")]
        [ProducesResponseType(typeof(RequestResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(RequestResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(GenericErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RevokeAccessToken(string accesstoken)
        {
            try
            {
                var result = await _userService.RevokenUserAccessToken(accesstoken);
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

        [HttpPost]
        [Route("refresh")]
        [ProducesResponseType(typeof(RequestResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(RequestResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(GenericErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenReq req)
        {
            try
            {

                var validation = await _customValidator.ValidateAsync(req);

                if (!validation.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, _customValidator.GetErrorResult(validation));
                }

                var result = await _userService.RefreshAccessToken(req.expiredToken);
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
