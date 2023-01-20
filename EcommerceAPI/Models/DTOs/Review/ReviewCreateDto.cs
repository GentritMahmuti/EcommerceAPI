using System.ComponentModel.DataAnnotations.Schema;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Models.DTOs.Review
{
    public class ReviewCreateDto
    {

        [ForeignKey("User")]
        public string UserId { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string ReviewComment { get; set; }
    }
}
