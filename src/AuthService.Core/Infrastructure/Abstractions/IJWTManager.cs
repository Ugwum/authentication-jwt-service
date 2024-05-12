using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Core.Infrastructure.Abstractions
{
    public interface IJWTManager
    {
        Task<string> CreateJWTToken(List<Claim> claims, DateTime? notbefore, DateTime? tokenExpiry);
        Task<(ClaimsPrincipal claimsPrincipal, SecurityToken securityToken)> ValidateAndReadTokenAsync(string token, bool validateLifeTime = true);

        Task RevokeToken(string token);

        Task<bool> IsTokenRevoked(string token);
    }
}
