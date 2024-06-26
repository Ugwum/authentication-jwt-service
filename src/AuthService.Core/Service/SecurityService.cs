﻿using Microsoft.Extensions.Logging;
using AuthService.Core.DataAccess.Repository;
using AuthService.Core.Infrastructure.Abstractions;
using AuthService.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Core.Service
{
    public interface ISecurityService
    {
        Task<RequestResult> RetrievePublicKey(string? clientId);

        Task<RequestResult> CalculateSignature(string? clientId);
    }
    public class SecurityService : ISecurityService
    {
        private readonly ILogger<SecurityService> _logger;
        private readonly IKeyPairStore _keyPairStore;
        private readonly AuthClientRepository _authClientRepo;


        public SecurityService(ILogger<SecurityService> logger,
            IKeyPairStore keyPairStore, AuthClientRepository authClientRepo)
        {
              _logger = logger;   
             _keyPairStore = keyPairStore;
            _authClientRepo = authClientRepo;
        }

      
        public async Task<RequestResult> RetrievePublicKey(string clientId)
        {
            try 
            {
                var authClient = await _authClientRepo.GetAuthClientAsync(clientId);

                if (authClient == null) { throw new CustomException("INVALID_AUTHCLIENT", "Client is invalid"); }

                var publickeyEncrypted = AESCryptoProviderExtension.Encrypt(await _keyPairStore.RetrievePublicKeyPair(), authClient.secretKey);

                return new RequestResult
                {
                    code = "Successful",
                    data = new { pubkey = publickeyEncrypted },
                    message = "Operation successful ",
                    Succeeded = true
                };

            }
            catch(CustomException ex)
            {
                _logger.LogError($"An error occurred. {ex.code}, {ex.Message}, {ex.StackTrace}");
                return new RequestResult { code = ex.code, data = null, message = ex.Message, Succeeded = false };

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred, {ex.Message}, {ex.StackTrace}");
                return new RequestResult { code = GeneralStatusCodes.UNEXPECTED_ERROR.code, data = null, message = GeneralStatusCodes.UNEXPECTED_ERROR.message, Succeeded = false };
            }
        }

        public async Task<RequestResult> CalculateSignature(string? clientId)
        {
            try
            {
                var authClient = await _authClientRepo.GetAuthClientAsync(clientId);

                if (authClient == null) { throw new CustomException("INVALID_AUTHCLIENT", "Client is invalid"); }

                var signature = AESCryptoProviderExtension.Encrypt(clientId, authClient.secretKey);

                return new RequestResult
                {
                    code = "Successful",
                    data = new { signature = signature },
                    message = "Operation successful ",
                    Succeeded = true
                };

            }
            catch (CustomException ex)
            {
                _logger.LogError($"An error occurred. {ex.code}, {ex.Message}, {ex.StackTrace}");
                return new RequestResult { code = ex.code, data = null, message = ex.Message, Succeeded = false };

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred, {ex.Message}, {ex.StackTrace}");
                return new RequestResult { code = GeneralStatusCodes.UNEXPECTED_ERROR.code, data = null, message = GeneralStatusCodes.UNEXPECTED_ERROR.message, Succeeded = false };
            }
        }

    }
}
