﻿using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Domain.Entities
{
    public class User
    {
        public static ClaimsIdentity Identity { get; internal set; }
        public string Id { get; set; }
        public string FirsName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public ICollection<Review> SubmittedReviews { get; set; }
    }
}
