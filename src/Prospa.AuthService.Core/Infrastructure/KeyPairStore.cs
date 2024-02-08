using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Prospa.AuthService.Core.Model.Configuration;
using Prospa.AuthService.Core.Infrastructure.Abstractions;

namespace Prospa.AuthService.Core.Infrastructure
{

    public class RSAKeyPairStore : IKeyPairStore
    {
        private readonly ILogger<RSAKeyPairStore> _logger;
        private readonly AppSettings _appSettings;
        private readonly string _certificateFilePath;
        private readonly string _certificatePassword;

        public RSAKeyPairStore(ILogger<RSAKeyPairStore> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings.Value;

            _certificateFilePath = appSettings.Value.CertificateFilePath;
            _certificatePassword = appSettings.Value.CertificatePassword;
        }

        public async Task<string> RetrievePublicKeyPair()
        {
            var secretKeyValue = _appSettings.PublicKeyPair;//await RetrieveKeyFromSecretsManager(privateSecretName);
            return secretKeyValue;
        }

        public async Task<string> RetrievePrivateKeyPair()
        {
            var secretKeyValue = _appSettings.PrivateKeyPair;//await RetrieveKeyFromSecretsManager(privateSecretName);
            return secretKeyValue;
        }
        

        private RSACryptoServiceProvider GetPublicKeyProvider()
        {
            var certificate = GetCertificate();
            var publicKey = (RSACryptoServiceProvider)certificate.PublicKey.Key;
            return publicKey;
        }

        private RSACryptoServiceProvider GetPrivateKeyProvider()
        {
            var certificate = GetCertificate();
            var privateKey = (RSACryptoServiceProvider)certificate.PrivateKey;
            return privateKey;
        }

        public X509Certificate2 GetCertificate()
        {
            try
            {
                return new X509Certificate2(_certificateFilePath, _certificatePassword);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred loading the certificate {ex.Message},{ex.StackTrace}");
                throw;
            }
        }
    }

}
