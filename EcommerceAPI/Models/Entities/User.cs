using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.Entities
{
    public class User
    {
        public string Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirsName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [EmailAddress]
        [Required]
        [MaxLength()]
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public ICollection<Review> SubmittedReviews { get; set; }
    }
}
