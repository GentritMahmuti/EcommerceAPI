using Services.Services.IServices;
using StackExchange.Redis;
using System.Text.Json;

namespace Services.Services
{
    public class CacheService : ICacheService
    {
        private IDatabase _cacheDb;

        public CacheService()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _cacheDb = redis.GetDatabase();
        }
        public T GetData<T>(string key)
        {
            var value = _cacheDb.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        public List<T> GetDataSet<T>(string key)
        {
            var value = _cacheDb.SetMembers(key);
            List<T> list = new List<T>();
            foreach (var item in value)
            {
                list.Add(JsonSerializer.Deserialize<T>(item));
            }
            return list;
        }
        public object RemoveData(string key)
        {
            var _exist = _cacheDb.KeyExists(key);
            if (_exist)
            {
                return _cacheDb.KeyDelete(key);
            }
            return false;
        }

        public object RemoveDataFromSet<T>(string key, T value)
        {
            var _exist = _cacheDb.KeyExists(key);
            if (_exist)
            {
                return _cacheDb.SetRemove(key, JsonSerializer.Serialize(value));
            }
            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);

            return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expiryTime);
        }


        public bool SetDataMember<T>(string key, T value)
        {

            return _cacheDb.SetAdd(key, JsonSerializer.Serialize(value));
        }


        public bool SetUpdatedData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expiryTime);
        }
    }
}
