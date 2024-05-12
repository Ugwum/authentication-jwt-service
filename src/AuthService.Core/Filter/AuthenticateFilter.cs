using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using AuthService.Core.Infrastructure;
using AuthService.Core.Infrastructure.Abstractions;
using AuthService.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Core.Filter
{
    public class AuthClientAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly ILogger<AuthClientAuthorizationFilter> _logger;
        private readonly IAuthClientManager _authClientManager;

        public AuthClientAuthorizationFilter(ILogger<AuthClientAuthorizationFilter> logger, IAuthClientManager authClientManager)
        {
            _logger = logger;
            _authClientManager = authClientManager; 
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                var allowAnonymous = context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType().Name == "AllowAnonymousAttribute");
                if (allowAnonymous)
                    return;

                var clientId = context.HttpContext.Request.Headers["ClientID"].FirstOrDefault()?.Split(" ").Last();
                var signature = context.HttpContext.Request.Headers["Signature"].FirstOrDefault()?.Split(" ").Last();

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(signature))
                {
                    context.Result = new JsonResult(new { code = "INVALID_AUTHCLIENT_CREDENTIAL", message = "ClientID or Signature missing in the header" })
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                    return;
                }

                var authClientIsValid = await _authClientManager.VerifyAuthServiceClient(clientId, signature);

                if (!authClientIsValid)
                {
                    context.Result = new JsonResult(new { code = "INVALID_AUTHCLIENT", message = "Auth client is not valid" })
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden
                    };
                    return;
                }

            }
            catch (CustomException ex)
            {
                _logger.LogError($"An error occurred Code {ex.code}, {ex.Message}, {ex.StackTrace}");
                context.Result = await HandleCustomExceptionAsync(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred {ex.Message}, {ex.StackTrace}");
                context.Result = await HandleExceptionAsync(ex);
            }
        }

        private static async Task<JsonResult> HandleCustomExceptionAsync(CustomException exception)
        {
            var data = new RequestResult { message = exception.Message, code = exception.code };

            return new JsonResult(data)
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
        }

        private static async Task<JsonResult> HandleExceptionAsync(Exception exception)
        {
            var data = new RequestResult { message = GeneralStatusCodes.UNEXPECTED_ERROR.message, code = GeneralStatusCodes.UNEXPECTED_ERROR.code };

            return new JsonResult(data)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }
}
