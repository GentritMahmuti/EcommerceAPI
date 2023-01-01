using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models.Entities
{
    public class Review
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Rating { get; set; }
        public string ReviewComment { get; set; }
        public DateTime ReviewPostedDate { get; set; }
    }
}
