using Prospa.AuthService.Core.Model;

namespace Prospa.AuthService.Core.Service.Abstractions
{
    public interface IUserService
    {
        Task<RequestResult> AuthenticateUser(string username, string password);
        Task<RequestResult> GetAuthUser(string? accesstoken);
        Task<RequestResult> RefreshAccessToken(string expiredToken);
        Task<RequestResult> RevokenUserAccessToken(string accesstoken);
    }
}
