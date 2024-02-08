using Prospa.AuthService.Core.Model;

namespace Prospa.AuthService.Core.Service.Abstractions
{
    public interface IJWTService
    {
        Task<string> GenerateJWT(string usertype, string username);

        Task<DecodedTokenDetail> DecodeJWTAsync(string authToken);

        Task<bool> IsTokenValidAsync(string token);

        Task<(bool isValid, DecodedTokenDetail tokendetails)> ValidateTokenAsync(string token);

        Task RevokeJWT(string jwtToken);


    }
}
