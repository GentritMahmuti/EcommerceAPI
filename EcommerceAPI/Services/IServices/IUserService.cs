using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUser(string id);
        Task DeleteUser(string id);
    }
}
