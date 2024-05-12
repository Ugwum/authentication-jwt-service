namespace AuthService.Core.Infrastructure.Abstractions
{
    public interface ICacheProvider
    {
        StackExchange.Redis.IDatabase GetDatabase();
        StackExchange.Redis.IDatabase GetDatabase(int dbId);
        Task SetStringListAsync(string key, List<string> values, double expiryMins);
        bool ItemExistInList(string listkey, string item);

        Task SetListAsync<T>(string key, List<T> objects, double expiryMins);
        Task<T> GetAsync<T>(string key);

        Task SetAsync<T>(string key, T value, double expiry);

        Task SetAsync<T>(string key, T value);

        void Set<T>(string key, T value);

        void Set<T>(string key, T value, double expiry);

        T Get<T>(string cacheKey);
        Task RemoveAsync<T>(string key);

        void Remove<T>(string key);
    }
}
