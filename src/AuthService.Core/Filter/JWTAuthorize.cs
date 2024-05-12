using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using AuthService.Core.Infrastructure;
using AuthService.Core.Infrastructure.Abstractions;
using AuthService.Core.Model;
using AuthService.Core.Service.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Core.Filter
{
    public class JWTAuthorize : IAsyncAuthorizationFilter
    {
        private readonly IJWTService _jwtService;
        private readonly ILogger<JWTAuthorize> _logger;
        public JWTAuthorize(IJWTService jwtService, ILogger<JWTAuthorize> logger)
        {
             _jwtService = jwtService;
            _logger = logger;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                var allowAnonymous = context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType().Name == "AllowAnonymousAttribute");
                if (allowAnonymous)
                    return;

                string? authorizationToken = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                var tokenIsValid = await _jwtService.IsTokenValidAsync(authorizationToken);

 
                if (!tokenIsValid)
                {
                    context.Result = new JsonResult(new { code = AuthStatusCode.INVALID_AUTHTOKEN.code, message = AuthStatusCode.INVALID_AUTHTOKEN.message })
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

        #region Helper method To be refactored
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

        #endregion

    }
}
