using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUser(string id);

        Task UpdateUser(UserDto userToUpdate);
        Task DeleteUser(string id);
    }
}
