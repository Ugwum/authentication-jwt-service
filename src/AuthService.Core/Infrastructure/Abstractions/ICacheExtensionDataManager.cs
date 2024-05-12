namespace AuthService.Core.Infrastructure.Abstractions
{
    public interface ICacheExtensionDataManager<T> where T : class
    {
        T Get(string key, Func<object[], T> fetchFromDatabase, params object[] inputParams);

        void Refresh(string key, Func<object[], T> fetchFromDatabase, params object[] inputParams);

        void Refresh(string key, Func<object[], T> onRefresh, double? cacheExpiryMins, params object[] inputParams);

        void Refresh(string key, Func<T> fetchFromDatabase);

        void Refresh(string key, Func<T> fetchFromDatabase, double? cacheExpiryMins);

        T Get(string key, Func<T> fetchFromDatabase);

        T Get(string key, Func<T> fetchFromDatabase, double? cacheExpiryMins);

        Task<T> GetAsync(string key, Func<Task<T>> fetchFromDatabase);

        Task<T> GetAsync(string key, Func<Task<T>> fetchFromDatabase, double? cacheExpiryMins);

        Task<T> GetAsync(string key, Func<object[], Task<T>> fetchFromDatabase, params object[] inputParams);

        Task<T> GetAsync(string key, Func<object[], Task<T>> fetchFromDatabase, double? cacheExpiryMins, params object[] inputParams);


    }
}
