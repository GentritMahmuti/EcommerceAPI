using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcommerceAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;


        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Gets all the users from db.
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetAllUsers()
        {
            var users = _unitOfWork.Repository<User>().GetAll();
            _logger.LogInformation($"{nameof(UserService)} - Got all users from db.");
            return users.ToList();
        }


        /// <summary>
        /// Gets a specific user by id if it exists, else throws Exception.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<User> GetUser(string id)
        {
            var user = await _unitOfWork.Repository<User>().GetById(x => x.Id.Equals(id)).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new Exception("A user with this ID doesn't exist.");
            }
            _logger.LogInformation($"{nameof(UserService)} - Got a user");
            return user;
        }


        /// <summary>
        /// Updates a specific user by id or throws Exception if it doesn't exist.
        /// </summary>
        /// <param name="userToUpdate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task UpdateUser(UserDto userToUpdate)
        {
            User? user = await GetUser(userToUpdate.UserId);

            if (user == null)
            {
                throw new Exception("A user with this ID doesn't exist.");
            }

            user.FirsName = userToUpdate.FirsName;
            user.LastName = userToUpdate.LastName;
            user.Email = userToUpdate.Email;
            user.Gender = userToUpdate.Gender;
            user.PhoneNumber = userToUpdate.PhoneNumber;


            _unitOfWork.Repository<User>().Update(user);

            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Deletes a specific user by id or throws Exception if it doesn't exist.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task DeleteUser(string id)
        {
            var user = await GetUser(id);
            if (user == null)
            {
                throw new Exception("A user with this ID doesn't exist.");
            }
            _unitOfWork.Repository<User>().Delete(user);
            await _unitOfWork.CompleteAsync();
        }
    }
}
