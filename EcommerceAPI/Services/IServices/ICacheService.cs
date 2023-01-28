
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface ICacheService
    {
        T GetData<T>(string key);
        bool SetData<T>(string key, T value, DateTimeOffset expirationTime);
        object RemoveData(string key);
        bool SetUpdatedData<T>(string key, T value, DateTimeOffset expirationTime);

    }
}
