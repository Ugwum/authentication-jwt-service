using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AuthService.Core.Infrastructure.Abstractions;
using StackExchange.Redis;

namespace AuthService.Core.Infrastructure
{
    public class RedisCacheProvider : ICacheProvider
    {
        private readonly IConnectionMultiplexer _connectionMutiplexer;
        public RedisCacheProvider(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMutiplexer = connectionMultiplexer;
        }
        public StackExchange.Redis.IDatabase GetDatabase(int dbId)
        {
            return _connectionMutiplexer.GetDatabase(dbId);
        }
        public StackExchange.Redis.IDatabase GetDatabase()
        {
            return _connectionMutiplexer.GetDatabase();
        }
        public async Task<T> GetAsync<T>(string cacheKey)
        {
            var db = _connectionMutiplexer.GetDatabase();

            var resultString = await db.StringGetAsync(cacheKey);

            T result = default(T);

            if (!resultString.IsNullOrEmpty)
            {
                result = JsonConvert.DeserializeObject<T>(resultString);
            }

            return result;
        }
        public async Task SetAsync<T>(string key, T value, double expiryMinutes)
        {
            var db = _connectionMutiplexer.GetDatabase();
            var serializedData = JsonConvert.SerializeObject(value);
            await db.StringSetAsync(key, serializedData, TimeSpan.FromMinutes(expiryMinutes));
            await db.KeyExpireAsync(key, TimeSpan.FromMinutes(expiryMinutes));
        }

        public async Task SetAsync<T>(string key, T value)
        {
            var db = _connectionMutiplexer.GetDatabase();
            var serializedData = JsonConvert.SerializeObject(value);
            await db.StringSetAsync(key, serializedData);
        }

        public async Task SetListAsync<T>(string key, List<T> objects, double expiryMins)
        {
            if (objects.Count > 0)
            {
                var database = GetDatabase();

                var expiry = TimeSpan.FromMinutes(expiryMins);

                database.KeyDelete(key);
                RedisValue[] values = objects.Select(x => (RedisValue)(JsonConvert.SerializeObject(x))).ToArray();

                database.SetAdd(key, values);
                database.KeyExpire(key, expiry);
            }
        }

        public async Task SetStringListAsync(string key, List<string> values, double expiryMins)
        {
            if (values != null && values.Count > 0)
            {
                var database = GetDatabase();
                var expiry = TimeSpan.FromMinutes(expiryMins);
                database.KeyDelete(key);
                RedisValue[] redisValues = values.Where(x => x != null).Select(x => (RedisValue)x).ToArray();
                await database.SetAddAsync(key, redisValues);
                await database.KeyExpireAsync(key, expiry);
            }
        }

        //public async Task SetStringListAsync(string key, List<string> values , double expiryMins)
        //{
        //    if (values.Count > 0)
        //    {
        //        var database = GetDatabase();

        //        var expiry = TimeSpan.FromMinutes(expiryMins); 
        //        database.KeyDelete(key);

        //        database.SetAdd(key, values.Select(x => (RedisValue)x).ToArray());
        //        database.KeyExpire(key, expiry);                  
        //    }
        //}

        public bool ItemExistInList(string listkey, string item)
        {
            var database = GetDatabase();
            var found = database.SetContains(listkey, item);
            return found;
        }

        public void Set<T>(string key, T value)
        {
            var db = _connectionMutiplexer.GetDatabase();
            var serializedData = JsonConvert.SerializeObject(value);
            db.StringSet(key, serializedData);
        }

        public void Set<T>(string key, T value, double expiry)
        {
            var db = _connectionMutiplexer.GetDatabase();
            var serializedData = JsonConvert.SerializeObject(value);
            db.StringSet(key, serializedData, TimeSpan.FromMinutes(expiry));
            db.KeyExpire(key, TimeSpan.FromMinutes(expiry));
        }

        //public void Set<T>(string key, T value, double expiry)
        //{

        //    //var db = _connectionMutiplexer.GetDatabase();
        //    //var serializedData = JsonConvert.SerializeObject(value);
        //    //await db.StringSet(key, serializedData);

        //    ////var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(expiry));
        //    ////var serializedData = JsonConvert.SerializeObject(value);
        //    ////_cache.Set(key, serializedData.SerializeToByteArray(), options);
        //}


        public T Get<T>(string cacheKey)
        {

            var db = _connectionMutiplexer.GetDatabase();

            var resultString = db.StringGet(cacheKey);

            T result = default(T);

            if (!resultString.IsNullOrEmpty)
            {
                result = JsonConvert.DeserializeObject<T>(resultString);
            }


            return result;


        }
        public async Task RemoveAsync<T>(string key)
        {

            var db = _connectionMutiplexer.GetDatabase();
            await db.KeyDeleteAsync(key);

        }

        public void Remove<T>(string key)
        {

            var db = _connectionMutiplexer.GetDatabase();
            db.KeyDelete(key);

        }

        //public async Task RemoveAsync(string key)
        //{
        //    await _cache.RemoveAsync(key);
        //}
    }
}
