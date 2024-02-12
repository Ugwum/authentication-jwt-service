using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prospa.AuthService.Core.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Prospa.AuthService.Core.Model.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Prospa.AuthService.Core.Model;

namespace Prospa.AuthService.Core.Infrastructure
{
    public class JWTManager : IJWTManager
    {
        private readonly X509Certificate2 _certificate;
        private readonly IKeyPairStore _keyPairStore;
        private readonly JWTSettings _jwtSettings;
        private readonly ILogger<JWTManager> _logger;
        private readonly ICacheProvider _cacheProvider;
        public JWTManager(IKeyPairStore keyPairStore,
            IOptions<JWTSettings> jwtOptions, ILogger<JWTManager> logger, 
            ICacheProvider cacheProvider)
        {
            _jwtSettings = jwtOptions.Value;
            _keyPairStore = keyPairStore;
            //_certificate = keyPairStore.GetCertificate();
            _logger = logger;
            _cacheProvider = cacheProvider;
        }
        public async Task<string> CreateJWTToken(List<Claim> claims, DateTime? notbefore, DateTime? tokenExpiry)
        {

            using (RSACryptoServiceProvider privateKey = new RSACryptoServiceProvider())
            {

                privateKey.ImportFromPem(await _keyPairStore.RetrievePrivateKeyPair());

                var rsaPrivateKey = new RsaSecurityKey(privateKey);

                var signingCredentials = new SigningCredentials(new RsaSecurityKey(privateKey), SecurityAlgorithms.RsaSha256)
                {
                    CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }

                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = new JwtSecurityToken(
                     _jwtSettings.JWTIssuer,
                     _jwtSettings.JWTAudience,
                     claims,
                     notbefore,
                     tokenExpiry,
                     signingCredentials
                 );
                var header = jwtSecurityToken.Header;
                header.Add("kid", Guid.NewGuid().ToString()); // Set the Key ID here

                var token = tokenHandler.WriteToken(jwtSecurityToken);
                return token;

            }

        }


        public async Task<(ClaimsPrincipal claimsPrincipal, SecurityToken securityToken)> ValidateAndReadTokenAsync(string token, bool validateLifeTime = true)
        {
            SecurityToken validatedToken = null;
            try
            {
                if (token == null)
                    throw new CustomException(AuthStatusCode.INVALID_AUTHTOKEN.code,AuthStatusCode.INVALID_AUTHTOKEN.message);

               if (await IsTokenRevoked(token)) throw new CustomException("TOKEN_REVOKED", $"token revoked");

                using (RSACryptoServiceProvider publickey = new RSACryptoServiceProvider())
                {

                    publickey.ImportFromPem(await _keyPairStore.RetrievePublicKeyPair());

                    var rsapublicKey = new RsaSecurityKey(publickey);

                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

                    var tokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateLifetime = validateLifeTime,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = rsapublicKey,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireSignedTokens = true,
                        ClockSkew = TimeSpan.Zero,

                    };

                    var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);

                    return (claimsPrincipal, validatedToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred, {ex.Message}, {ex.StackTrace}");
                throw ex;
            }
        }

        private async Task<bool> IsTokenRevoked(string token)
        {
            var cacheKey = "RevokedTokens";
            var revokedTokens = await _cacheProvider.GetAsync<List<string>>(cacheKey);

            var isRevoked = revokedTokens?.Contains(token) ?? false;

            return isRevoked;
        }

        public async Task RevokeToken(string token)
        {
            try
            {
                if (!await IsTokenRevoked(token))
                {
                    AddTokenToRevokedList(token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred {ex.Message}, {ex.StackTrace}");
                throw ex;
            }
        }

        private void AddTokenToRevokedList(string token)
        {
            try
            {
                var cacheKey = "RevokedTokens";
                var revokedTokens = _cacheProvider.Get<List<string>>(cacheKey) ?? new List<string>();

                revokedTokens.Add(token);

                _cacheProvider.Set(cacheKey, revokedTokens);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An errror occurred,{ex.Message}, {ex.StackTrace}");
            }

        }

    }
}
