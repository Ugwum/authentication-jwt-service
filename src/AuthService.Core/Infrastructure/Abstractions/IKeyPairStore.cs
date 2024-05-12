using System.Security.Cryptography.X509Certificates;

namespace AuthService.Core.Infrastructure.Abstractions
{
    public interface IKeyPairStore
    {
        Task<string> RetrievePrivateKeyPair();
        Task<string> RetrievePublicKeyPair();

        X509Certificate2 GetCertificate();
    }

}
