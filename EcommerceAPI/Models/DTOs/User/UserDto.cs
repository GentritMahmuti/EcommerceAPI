
﻿using System.ComponentModel.DataAnnotations;
namespace EcommerceAPI.Models.DTOs.User
{
    public class UserDto
    {
        public string UserId { get; set; }
        public string FirsName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
}
