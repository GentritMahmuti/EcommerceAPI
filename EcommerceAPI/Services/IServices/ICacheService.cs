
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface ICacheService
    {
        List<T> GetData <T>(string key);
        bool SetData<T>(string key, T value, DateTimeOffset expirationTime);
        object RemoveData(string key);
        bool SetUpdatedData(string key, OrderDetails value, DateTimeOffset expirationTime);

    }
}
