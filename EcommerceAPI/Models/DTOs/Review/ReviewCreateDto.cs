using System.ComponentModel.DataAnnotations.Schema;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Models.DTOs.Review
{
    public class ReviewCreateDto
    {

        [ForeignKey("User")]
        public string UserId { get; set; }
        public Entities.User User { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Entities.Product Product { get; set; }
        public int Rating { get; set; }
        public string ReviewComment { get; set; }
        public DateTime ReviewPostedDate { get; set; }
    }
}
