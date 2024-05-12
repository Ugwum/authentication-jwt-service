namespace AuthService.Core.Infrastructure.Abstractions
{
    public interface IAuthClientManager
    {
        Task<bool> VerifyAuthServiceClient(string clientId, string signature);
    }
}
