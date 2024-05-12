using Microsoft.Extensions.Logging;
using AuthService.Core.DataAccess.Data;
using AuthService.Core.DataAccess.Repository;
using AuthService.Core.Infrastructure.Abstractions;
using AuthService.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Core.Infrastructure
{
    public class AuthClientManager : IAuthClientManager
    {
        private readonly ILogger<AuthClientManager> _logger;
        private readonly AuthClientRepository _authClientRepo;
        

        public AuthClientManager(ILogger<AuthClientManager> logger, AuthClientRepository authClientRepo)
        {
            _logger = logger;
            _authClientRepo = authClientRepo;
        }
        public async Task<bool> VerifyAuthServiceClient(string clientId, string signature)
        {
            try
            {
                var authClient = await _authClientRepo.GetAuthClientAsync(clientId);

                if (authClient == null) { throw new CustomException(AuthStatusCode.INVALID_AUTHCLIENT.code, AuthStatusCode.INVALID_AUTHCLIENT.message); }

                return  VerifyClientSignature(authClient, signature);
                 
            }
            catch (CustomException ex)
            {
                _logger.LogError($"An error occurred, Code {ex.code} {ex.Message}, {ex.StackTrace}");
                throw ex;                
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred, {ex.Message}, {ex.StackTrace}");
                throw ex;              

            }
        }
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authClient"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        private bool VerifyClientSignature(AuthClient authClient, string signature)
        {
            try
            {
                var secretkeybytes = Convert.FromBase64String(authClient.secretKey);


                var encodedSignatureBytes = Convert.FromBase64String(signature); //Encoding.UTF8.GetBytes(signature);
                var clientId = AESCryptoProviderExtension.Decrypt(encodedSignatureBytes, secretkeybytes);

                return clientId == authClient.secretId;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
