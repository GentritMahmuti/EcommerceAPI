﻿using EcommerceAPI.Data.UnitOfWork;
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

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<User>> GetAllUsers()
        {
            var users = _unitOfWork.Repository<User>().GetAll();

            return users.ToList();
        }

        public async Task<User> GetUser(string id)
        {
            Expression<Func<User, bool>> expression = x => x.Id.Equals(id);
            var user = await _unitOfWork.Repository<User>().GetById(expression).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new Exception("A user with this ID doesn't exist.");
            }

            return user;
        }

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

            _unitOfWork.Complete();
        }
        public async Task DeleteUser(string id)
        {

            var user = await GetUser(id);
            if (user == null)
            {
                throw new Exception("A user with this ID doesn't exist.");
            }
            _unitOfWork.Repository<User>().Delete(user);
            _unitOfWork.Complete();
        }
    }
}
