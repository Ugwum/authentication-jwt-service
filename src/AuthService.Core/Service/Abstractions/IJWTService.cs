using AuthService.Core.Model;

namespace AuthService.Core.Service.Abstractions
{
    public interface IJWTService
    {
        Task<string> GenerateJWT(string usertype, string username);

        Task<DecodedTokenDetail> DecodeJWTAsync(string authToken);

        Task<bool> IsTokenValidAsync(string token);

        Task<(bool isValid, DecodedTokenDetail tokendetails)> ValidateTokenAsync(string token);

        Task RevokeJWT(string jwtToken);

        Task<bool> IsTokenRevokedAsync(string token);


    }
}
