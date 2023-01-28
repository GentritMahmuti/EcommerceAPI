using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models.DTOs.Review
{
    public class ReviewUpdateDto
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string ReviewComment { get; set; }
    }
}
