
using EcommerceAPI.Models.Entities;
using StackExchange.Redis;

namespace EcommerceAPI.Services.IServices
{
    public interface ICacheService
    {
        T GetData<T>(string key);
        List<T> GetDataSet<T>(string key);
        bool SetData<T>(string key, T value, DateTimeOffset expirationTime);
        bool SetDataMember<T>(string key, T value);
        object RemoveData(string key);
        bool SetUpdatedData<T>(string key, T value, DateTimeOffset expirationTime);

    }
}
