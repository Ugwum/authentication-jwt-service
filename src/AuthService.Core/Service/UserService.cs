﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using AuthService.Core.DataAccess.Abstractions;
using AuthService.Core.DataAccess.Data;
using AuthService.Core.DataAccess.Repository;
using AuthService.Core.Model;
using AuthService.Core.Model.Configuration;
using AuthService.Core.Service.Abstractions;
using AuthService.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Core.Service
{
    public class UserService : IUserService
    {
        public readonly IJWTService _jwtService;
        private readonly ILogger<UserService> _logger;
       // private readonly IRepositoryAsync<CustomUser> _customUserRepo;
        private readonly IGuard _guard;
        private readonly AppSettings _appSettings;
        private readonly CustomUserRepository _customUserRepo;
        public UserService(IJWTService jwtService, ILogger<UserService> logger,
            CustomUserRepository customUserRepo, IGuard guard, IOptions<AppSettings> appSettings)
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

                _guard.Against(!userValidation.isValid, UserStatusCodes.INVALID_CREDENTIALS.code, UserStatusCodes.INVALID_CREDENTIALS.message, _logger, true);

                _logger.LogInformation($"Generating token for user name {username}");
                var token = await _jwtService.GenerateJWT(userValidation.userobj.user_type, username);

                var userVM = Transform.From(userValidation.userobj);
                userVM.accesstoken = token;

                

                return new RequestResult
                {
                    Succeeded = true,
                    code = "00",
                    data = userVM,
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


        public async Task<RequestResult> GetAuthUser(string? accesstoken)
        {
            try
            {
                _logger.LogInformation($"Retrieving auth user details for accesstoken {accesstoken}");
                var tokendetails = await _jwtService.ValidateTokenAsync(accesstoken);

                _guard.Against(tokendetails.isValid == false, $"Invalid token", _logger, true);

                var customUser = await _customUserRepo.GetCustomUserAsync(tokendetails.tokendetails.username.ToLower());

                if (customUser == null) throw new CustomException(UserStatusCodes.INVALID_USER.code, UserStatusCodes.INVALID_USER.message);
                 
                var response = new { firstname = customUser.first_Name, lastname = customUser.last_Name, 
                    email = customUser.email, customUser.date_Joined, uuid =  customUser.id, customUser.is_Active, customUser.phone };

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
                _logger.LogError($"An error occurred, {ex.Message}, {ex.StackTrace} {ex.code}");
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
                _logger.LogInformation($"Refreshing access token user details for accesstoken {expiredToken}");
                var tokendetails = await _jwtService.DecodeJWTAsync(expiredToken);

                _guard.Against(tokendetails == null, $"Invalid token", _logger, true);

                var customUser = _customUserRepo.GetCustomUserAsync(tokendetails.username.ToLower());

                if (customUser == null) throw new CustomException(UserStatusCodes.INVALID_USER.code, UserStatusCodes.INVALID_USER.message);

                var expiry = RSACryptoProviderExtension.DecryptWithPrivateKey(tokendetails.refexp, _appSettings.PrivateKeyPair);

                if (Convert.ToDateTime(expiry) < DateTime.UtcNow) throw new CustomException(AuthStatusCode.LOGIN_REQUIRED.code, AuthStatusCode.LOGIN_REQUIRED.message);

                var token = await _jwtService.GenerateJWT(tokendetails.usertype, tokendetails.username);

                var response = new  { accesstoken = token/*, refreshtoken = token*/ };

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
                _logger.LogInformation($"Revoking access token {accesstoken}");

                var tokendetails = await _jwtService.DecodeJWTAsync(accesstoken);

                _guard.Against(tokendetails == null, $"Invalid token", _logger, true);

                await _jwtService.RevokeJWT(accesstoken);

                return new RequestResult
                {
                    Succeeded = true,
                    code = "00",
                    data = null,
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

        public async Task<RequestResult> IntrospectAccessToken(string accesstoken)
        {
            try
            {
                _logger.LogInformation($"Revoking access token {accesstoken}");

                var tokendetails = await _jwtService.DecodeJWTAsync(accesstoken);

                _guard.Against(tokendetails == null, $"Invalid token", _logger, true);

                var isTokenRevoken = await _jwtService.IsTokenRevokedAsync(accesstoken);

                var returndata = new
                {
                    tokenRevoked = isTokenRevoken,
                    usertype = tokendetails.usertype,
                    username = tokendetails.username,
                    expirydate = tokendetails.exp
                }; 

                return new RequestResult
                {
                    Succeeded = true,
                    code = "00",
                    data = returndata,
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

        private async Task<(bool isValid, LoginUserResponse userobj)> ValidateUserCredentials(string username, string password)
        {
            try
            {
                _logger.LogInformation($"Validating user credentials for username {username}");
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
