using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prospa.AuthService.Core.Infrastructure.Abstractions;
using Prospa.AuthService.Core.Model;
using Prospa.AuthService.Core.Model.Configuration;
using Prospa.AuthService.Core.Service.Abstractions;
using Prospa.AuthService.Core.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Prospa.AuthService.Core.Service
{
    public class JWTService : IJWTService
    {
        private readonly IOptions<JWTSettings> _jwtSettings;
        private readonly AppSettings _appSettings;
        private readonly IJWTManager _jwtManager;

      //  private readonly ICacheProvider _cacheProvider;
        private readonly ILogger<JWTService> _logger;

        public JWTService(IJWTManager jwtManager, IOptions<JWTSettings> jwtOptions,IOptions<AppSettings> appSettings, 
            /*ICacheProvider cacheProvider*/ ILogger<JWTService> logger)
        {
            _jwtManager = jwtManager;
            _jwtSettings = jwtOptions;
            _appSettings = appSettings.Value;
           // _cacheProvider = cacheProvider;
            _logger = logger;
        }


        public async Task<string> GenerateJWT(string usertype, string username)
        {
            try
            {
                var time = Convert.ToInt32(_jwtSettings.Value.JWTAccessTokenExpiry);
                var refreshtimeframe = DateTime.UtcNow.AddMinutes(Convert.ToInt32(Convert.ToInt32(_jwtSettings.Value.JWTRefreshTokenExpiry))).ToString();

                var claims = new List<Claim>()
                {
                    new Claim("username",username),
                    new Claim("usertype",usertype),
                    new Claim("aud", "access"),
                    new Claim("refexp", CryptoHelper.Encrypt(refreshtimeframe,_appSettings.PrivateKeyPair)),
                    new Claim("exp", DateTime.UtcNow.AddMinutes(Convert.ToInt32(time)).ToString()),
                };

                var accesstoken = await _jwtManager.CreateJWTToken(claims, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(time));

                return accesstoken.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred, {ex.Message}, {ex.StackTrace}");
                throw ex;
            }
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            try
            {
                if (token == null)
                    return false;

                var tokenresult = await _jwtManager.ValidateAndReadTokenAsync(token, true);
                return (tokenresult.securityToken != null && tokenresult.claimsPrincipal != null);

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred, {ex.Message}, {ex.StackTrace}");
                return false;
            }
        }


        public async Task<(bool isValid, DecodedTokenDetail tokendetails)> ValidateTokenAsync(string token)
        {
            DecodedTokenDetail tokenDetail = null;
            try
            {
                if (token == null)
                    return (false, tokenDetail);

                var tokenresult = await _jwtManager.ValidateAndReadTokenAsync(token, true);

                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenresult.securityToken;

                tokenDetail = new DecodedTokenDetail
                {
                    aud = jwtToken.Claims.First(x => x.Type == "aud").Value,// To differntiate Access from refresh token
                    username = jwtToken.Claims.First(x => x.Type == "username").Value,
                    usertype = jwtToken.Claims.First(x => x.Type == "usertype").Value,// Session id
                    iss = jwtToken.IssuedAt,
                    exp = jwtToken.ValidTo,
                    refexp = jwtToken.Claims.First(x => x.Type == "refexp").Value
                };

                return ((tokenresult.securityToken != null && tokenresult.claimsPrincipal != null), tokenDetail);

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred, {ex.Message}, {ex.StackTrace}");
                return (false, tokenDetail);
            }
        }

        public async Task<DecodedTokenDetail> DecodeJWTAsync(string authToken)
        {
            try
            {
                if (authToken == null)
                    return null;

                var tokenresult = await _jwtManager.ValidateAndReadTokenAsync(authToken, false);

                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenresult.securityToken;

                var DecodedToken = new DecodedTokenDetail
                {
                    aud = jwtToken.Claims.First(x => x.Type == "aud").Value,// To differntiate Access from refresh token
                    username = jwtToken.Claims.First(x => x.Type == "username").Value,
                    usertype = jwtToken.Claims.First(x => x.Type == "usertype").Value,// Session id
                    iss = jwtToken.IssuedAt,
                    exp = jwtToken.ValidTo,
                    refexp = jwtToken.Claims.First(x => x.Type == "refexp").Value
                };
                return DecodedToken;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred, {ex.Message}, {ex.StackTrace}");
                throw ex;
            }

        }

        public async Task RevokeJWT(string jwtToken)
        {
            await _jwtManager.RevokeToken(jwtToken);
        }
    }
}
