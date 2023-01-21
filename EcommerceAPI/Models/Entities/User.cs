using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.Entities
{
    public class User
    {
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
