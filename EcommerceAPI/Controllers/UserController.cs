using EcommerceAPI.Services.IServices;
using EcommerceAPI.Validators.EntityValidators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.User;
using Services.Services.IServices;

namespace EcommerceAPI.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IValidator<UserDto> _userValidator;

        public UserController(IUserService userService, ILogger<UserController> logger, IValidator<UserDto> userValidator)
        {
            _userService = userService;
            _logger = logger;
            _userValidator = userValidator;
        }


        /// <summary>
        /// Admins can see users based on their ID.
        /// </summary>
        /// <param name="Userid"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeAdmin")]
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser(string Userid)
        {
            try
            {
                var user = await _userService.GetUser(Userid);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UserController)} - An error occured while trying to add a product to card");
                return BadRequest("An error happened: " + ex.Message);
            }
        }

        /// <summary>
        /// Admins can get all the users that exist in db.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "LifeAdmin")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        /// <summary>
        /// Updates a specific user by id.
        /// </summary>
        /// <param name="userToUpdate"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeAdmin")]
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UserDto userToUpdate)
        {
            try
            {
                await _userValidator.ValidateAndThrowAsync(userToUpdate);
                await _userService.UpdateUser(userToUpdate);
                _logger.LogInformation("Updating a user");
                return Ok("User updated successfully!");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error  updating user data");
                return BadRequest("An error happened:" + e.Message);
            }
        }

        /// <summary>
        /// Admins can delete a specific user by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeAdmin")]
        [HttpPost("DeleteUser")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _userService.DeleteUser(id);
                _logger.LogInformation("Deleting a user");
                return Ok("User deleted successfully!");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting user");
                return BadRequest("An error happened:" + e.Message);
            }
        }
    }
}
