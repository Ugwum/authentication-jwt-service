using Microsoft.Extensions.Logging;
using Prospa.AuthService.Core.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Prospa.AuthService.Core.Model;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Prospa.AuthService.Core.Middleware
{
    public class AuthClientRequestValidator
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthClientRequestValidator> _logger;
       // private readonly IAuthClientManager _authClientManager;
       

        public AuthClientRequestValidator(RequestDelegate next, ILogger<AuthClientRequestValidator> logger
            /*IAuthClientManager authClientManager*/)
        {
            _next = next;
            _logger = logger;
            //_authClientManager = authClientManager;
        }

        public async Task Invoke(HttpContext context, IServiceProvider serviceProvider)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {

                    if (!context.Request.Path.Value.Contains("calculate-signature"))
                    {
                        var authClientManager = scope.ServiceProvider.GetRequiredService<IAuthClientManager>();

                        var clientId = context.Request.Headers["ClientID"].FirstOrDefault()?.Split(" ").Last().ToString();
                        var signature = context.Request.Headers["Signature"].FirstOrDefault()?.Split(" ").Last().ToString();

                        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(signature)) { throw new CustomException($"INVALID_AUTHCLIENT_CREDENTIAL", "ClientID or Signature missing in the header"); }

                        var authClientIsValid = await authClientManager.VerifyAuthServiceClient(clientId, signature);

                        if (!authClientIsValid)
                        {
                            throw new CustomException($"INVALID_AUTHCLIENT", "Auth client is not valid");
                        }
                    }

                    await _next(context);
                }

            }
            catch(CustomException ex)
            {
                _logger.LogError($"An error occurred Code {ex.code}, {ex.Message}, {ex.StackTrace}");
                await HandleCustomExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred {ex.Message}, {ex.StackTrace}");
                await HandleExceptionAsync(context, ex);               
            }


        }

        private static Task HandleCustomExceptionAsync(HttpContext context, CustomException exception)
        {
            var data = new RequestResult { message = exception.Message, code = exception.code };            
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode =/* exception.code == "INVALID_AUTHCLIENT_CREDENTIAL" ? (int)HttpStatusCode.BadRequest :*/(int)HttpStatusCode.Forbidden;
            var result = JsonConvert.SerializeObject(data);
            return context.Response.WriteAsync(result);            
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var data = new RequestResult { message = GeneralStatusCodes.UNEXPECTED_ERROR.message, code = GeneralStatusCodes.UNEXPECTED_ERROR.code };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var result = JsonConvert.SerializeObject(data);
            return context.Response.WriteAsync(result);
        }

    }
}
