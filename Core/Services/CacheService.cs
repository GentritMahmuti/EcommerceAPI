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
        //Get data from redis
        public T GetData<T>(string key)
        {
            var value = _cacheDb.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }
        //Get data from redis set
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
        //Remove data from redis
        public object RemoveData(string key)
        {
            var _exist = _cacheDb.KeyExists(key);
            if (_exist)
            {
                return _cacheDb.KeyDelete(key);
            }
            return false;
        }
        //Remove data from redis set
        public object RemoveDataFromSet<T>(string key, T value)
        {
            var _exist = _cacheDb.KeyExists(key);
            if (_exist)
            {
                return _cacheDb.SetRemove(key, JsonSerializer.Serialize(value));
            }
            return false;
        }
        //Store data in redis
        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);

            return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expiryTime);
        }

        //Store data in redis set
        public bool SetDataMember<T>(string key, T value)
        {

            return _cacheDb.SetAdd(key, JsonSerializer.Serialize(value));
        }

        //Update data in chache
        public bool SetUpdatedData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expiryTime);
        }
    }
}
