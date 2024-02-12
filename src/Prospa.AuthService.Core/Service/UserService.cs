using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Prospa.AuthService.Core.DataAccess.Abstractions;
using Prospa.AuthService.Core.DataAccess.Data;
using Prospa.AuthService.Core.Model;
using Prospa.AuthService.Core.Model.Configuration;
using Prospa.AuthService.Core.Service.Abstractions;
using Prospa.AuthService.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Prospa.AuthService.Core.Service
{
    public class UserService : IUserService
    {
        public readonly IJWTService _jwtService;
        private readonly ILogger<UserService> _logger;
        private readonly IRepositoryAsync<CustomUser> _customUserRepo;
        private readonly IGuard _guard;
        private readonly AppSettings _appSettings;
        public UserService(IJWTService jwtService, ILogger<UserService> logger,
            IRepositoryAsync<CustomUser> customUserRepo, IGuard guard, IOptions<AppSettings> appSettings)
        {
            _jwtService = jwtService;
            _logger = logger;
            _customUserRepo = customUserRepo;
            _guard = guard;
            _appSettings = appSettings.Value;
        }

        public async Task<RequestResult> AuthenticateUser(string username, string password)
        {
            try
            {
                var userValidation = await ValidateUserCredentials(username, password);
                if (userValidation.isValid)
                {
                    var token = await _jwtService.GenerateJWT(userValidation.userobj.user_type, username);

                    var userVM = Transform.From(userValidation.userobj);
                    userVM.acceesstoken = token;

                    return new RequestResult
                    {
                        Succeeded = true,
                        code = "00",
                        data = userVM,
                        message = "Operation successfull"
                    };
                }
                else
                {
                    throw new CustomException(UserStatusCodes.INVALID_CREDENTIALS.code, UserStatusCodes.INVALID_CREDENTIALS.message);
                }
            }
            catch (CustomException ex)
            {
                _logger.LogError($"An error occurred, {ex.Message}, {ex.StackTrace}");
                return new RequestResult { code = ex.code, data = null, message = ex.Message, Succeeded = false };
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred, {ex.Message}, {ex.StackTrace}");
                return new RequestResult { code = GeneralStatusCodes.UNEXPECTED_ERROR.code, data = null, message = GeneralStatusCodes.UNEXPECTED_ERROR.message, Succeeded = false };
            }
        }

        public async Task<RequestResult> RefreshAccessToken(string expiredToken)
        {
            try
            {
                var tokendetails = await _jwtService.DecodeJWTAsync(expiredToken);

                _guard.Against(tokendetails == null, $"Invalid token", _logger, true);

                var customUser = _customUserRepo.Query(c => c.email.ToLower() == tokendetails.username.ToLower()).SingleOrDefault();

                if (customUser == null) throw new CustomException(UserStatusCodes.INVALID_USER.code, UserStatusCodes.INVALID_USER.message);

                var expiry = RSACryptoProviderExtension.DecryptWithPrivateKey(tokendetails.refexp, _appSettings.PrivateKeyPair);

                if (Convert.ToDateTime(expiry) < DateTime.UtcNow) throw new CustomException(AuthStatusCode.LOGIN_REQUIRED.code, AuthStatusCode.LOGIN_REQUIRED.message);

                var token = await _jwtService.GenerateJWT(tokendetails.usertype, tokendetails.username);

                var response = new UserVM { acceesstoken = token/*, refreshtoken = token*/ };

                return new RequestResult
                {
                    Succeeded = true,
                    code = "00",
                    data = response,
                    message = "Operation successfull"
                };
            }
            catch (CustomException ex)
            {
                _logger.LogError($"An error occurred, {ex.Message}, {ex.StackTrace}");
                return new RequestResult { code = ex.code, data = null, message = ex.Message, Succeeded = false };
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred, {ex.Message}, {ex.StackTrace}");
                return new RequestResult { code = GeneralStatusCodes.UNEXPECTED_ERROR.code, data = null, message = GeneralStatusCodes.UNEXPECTED_ERROR.message, Succeeded = false };
            }
        }

        public async Task<RequestResult> RevokenUserAccessToken(string accesstoken)
        {
            try
            {
                await _jwtService.RevokeJWT(accesstoken);

                return new RequestResult
                {
                    Succeeded = true,
                    code = "00",
                    data = null,
                    message = "Operation successfull"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred, {ex.Message}, {ex.StackTrace}");
                return new RequestResult { code = GeneralStatusCodes.UNEXPECTED_ERROR.code, data = null, message = GeneralStatusCodes.UNEXPECTED_ERROR.message, Succeeded = false };
            }
        }

        private async Task<(bool isValid, LoginUserResponse userobj)> ValidateUserCredentials(string username, string password)
        {
            try
            {

                var _retryPolicy = Policy.Handle<HttpRequestException>()
                                  .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                var result = await _retryPolicy.ExecuteAsync(async () => await CallProspaLoginAPI(username, password));
                
                var customUser = ProcessLoginResult(result);

                return (true, customUser);

            }
            catch (CustomException ex)
            {
                _logger.LogError($"An error occurred, {ex.Message}, {ex.StackTrace}");
                throw ex;
            }

            catch (Exception ex)
            {
                _logger.LogError($"An error occurred, {ex.Message}, {ex.StackTrace}");
                throw ex;
            }
        }

        private async Task<HttpResponseMessage> CallProspaLoginAPI(string username, string password)
        {
            using (var httpClient = new HttpClient())
            {
               var reqSerialized = JsonConvert.SerializeObject(new { username = username, password = password });

                HttpContent content = new StringContent(reqSerialized, Encoding.UTF8, "application/json");

                return await httpClient.PostAsync(_appSettings.ProspaLoginAPIUrl, content);
            }
        }

        private LoginUserResponse ProcessLoginResult(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                var loginUserResponse = JsonConvert.DeserializeObject<LoginUserResponse>(content);
                return loginUserResponse;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new CustomException(UserStatusCodes.INVALID_CREDENTIALS.code, UserStatusCodes.INVALID_CREDENTIALS.message);
            }
            else
            {
                throw new Exception("An error occurred");
            }
        }
    }
}
