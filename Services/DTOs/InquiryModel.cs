using System.ComponentModel.DataAnnotations;

namespace Services.DTOs
{
    public class InquiryModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
