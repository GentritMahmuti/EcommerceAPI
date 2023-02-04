

using Domain.Entities;
using Services.DTOs.User;

namespace Services.Services.IServices
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUser(string id);

        Task UpdateUser(UserDto userToUpdate);
        Task DeleteUser(string id);
    }
}
