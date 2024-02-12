using Microsoft.Extensions.Options;
using Prospa.AuthService.Core.Infrastructure.Abstractions;
using Prospa.AuthService.Core.Model.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prospa.AuthService.Core.Infrastructure
{
    public class CacheExtensionDataManager<T> : ICacheExtensionDataManager<T> where T : class
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly RedisConnection _redisConnection;
        private readonly double? _defaultCacheExpiry;

        public CacheExtensionDataManager(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
            _defaultCacheExpiry = Convert.ToDouble(90);
        }

        public void Refresh(string key, Func<object[], T> onRefresh, params object[] inputParams)
        {
            var dataFromRefresh = onRefresh(inputParams);

            if (dataFromRefresh != null) { _cacheProvider.Set<T>(key, dataFromRefresh, _defaultCacheExpiry.Value); }

        }

        public void Refresh(string key, Func<object[], T> onRefresh, double? cacheExpiryMins, params object[] inputParams)
        {
            var dataFromRefresh = onRefresh(inputParams);

            if (dataFromRefresh != null) { _cacheProvider.Set<T>(key, dataFromRefresh, cacheExpiryMins ?? _defaultCacheExpiry.Value); }

        }

        public void Refresh(string key, Func<T> fetchFromDatabase, double? cacheExpiryMins)
        {
            var dataFromRefresh = fetchFromDatabase();

            if (dataFromRefresh != null) { _cacheProvider.Set<T>(key, dataFromRefresh, cacheExpiryMins ?? _defaultCacheExpiry.Value); }
        }

        public void Refresh(string key, Func<T> fetchFromDatabase)
        {
            var dataFromRefresh = fetchFromDatabase();

            if (dataFromRefresh != null) { _cacheProvider.Set<T>(key, dataFromRefresh, _defaultCacheExpiry.Value); }
        }


        public T Get(string key, Func<T> fetchFromDatabase)
        {
            var cacheValue = _cacheProvider.Get<T>(key);

            if (cacheValue == null)
            {
                cacheValue = fetchFromDatabase();
                _cacheProvider.Set<T>(key, cacheValue, _defaultCacheExpiry.Value);
            }
            return cacheValue;
        }

        public T Get(string key, Func<T> fetchFromDatabase, double? cacheExpiryMins)
        {
            var cacheValue = _cacheProvider.Get<T>(key);

            if (cacheValue == null)
            {
                cacheValue = fetchFromDatabase();
                _cacheProvider.Set<T>(key, cacheValue, cacheExpiryMins ?? _defaultCacheExpiry.Value);
            }
            return cacheValue;
        }

        public async Task<T> GetAsync(string key, Func<Task<T>> fetchFromDatabase, double? cacheExpiryMins)
        {
            var cacheValue = await _cacheProvider.GetAsync<T>(key);

            if (cacheValue == null)
            {
                var dbData = await fetchFromDatabase();
                cacheValue = dbData;
                if (dbData != null) { await _cacheProvider.SetAsync<T>(key, cacheValue, cacheExpiryMins ?? _defaultCacheExpiry.Value); }

            }

            return cacheValue;
        }

        public async Task<T> GetAsync(string key, Func<Task<T>> fetchFromDatabase)
        {
            var cacheValue = await _cacheProvider.GetAsync<T>(key);

            if (cacheValue == null)
            {
                var dbData = await fetchFromDatabase();
                cacheValue = dbData;
                if (dbData != null) { await _cacheProvider.SetAsync<T>(key, cacheValue, _defaultCacheExpiry.Value); }
            }
            return cacheValue;
        }

        public T Get(string key, Func<object[], T> fetchFromDatabase, params object[] inputParams)
        {
            var cacheValue = _cacheProvider.Get<T>(key);

            if (cacheValue == null)
            {
                var dbData = fetchFromDatabase(inputParams);
                cacheValue = dbData;
                if (dbData != null) { _cacheProvider.Set<T>(key, cacheValue, _defaultCacheExpiry.Value); }

            }
            return cacheValue;
        }

        public T Get(string key, Func<object[], T> fetchFromDatabase, double? cacheExpiryMins, params object[] inputParams)
        {
            var cacheValue = _cacheProvider.Get<T>(key);

            if (cacheValue == null)
            {
                var dbData = fetchFromDatabase(inputParams);
                cacheValue = dbData;
                if (dbData != null) { _cacheProvider.Set<T>(key, cacheValue, cacheExpiryMins ?? _defaultCacheExpiry.Value); }

            }
            return cacheValue;
        }

        public async Task<T> GetAsync(string key, Func<object[], Task<T>> fetchFromDatabase, params object[] inputParams)
        {
            var cacheValue = await _cacheProvider.GetAsync<T>(key);

            if (cacheValue == null)
            {
                var dbData = await fetchFromDatabase(inputParams);
                cacheValue = dbData;
                if (dbData != null) { await _cacheProvider.SetAsync<T>(key, cacheValue, _defaultCacheExpiry.Value); }

            }

            return cacheValue;
        }
        public async Task<T> GetAsync(string key, Func<object[], Task<T>> fetchFromDatabase, double? cacheExpiryMins, params object[] inputParams)
        {
            var cacheValue = await _cacheProvider.GetAsync<T>(key);

            if (cacheValue == null)
            {
                var dbData = await fetchFromDatabase(inputParams);
                cacheValue = dbData;
                if (dbData != null) { await _cacheProvider.SetAsync<T>(key, cacheValue, cacheExpiryMins ?? _defaultCacheExpiry.Value); }

            }

            return cacheValue;
        }
    }
}
