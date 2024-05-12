using AuthService.Core.Model;

namespace AuthService.Core.Service.Abstractions
{
    public interface IUserService
    {
        Task<RequestResult> AuthenticateUser(string username, string password);
        Task<RequestResult> GetAuthUser(string? accesstoken);
        Task<RequestResult> IntrospectAccessToken(string accesstoken);
        Task<RequestResult> RefreshAccessToken(string expiredToken);
        Task<RequestResult> RevokenUserAccessToken(string accesstoken);
    }
}
