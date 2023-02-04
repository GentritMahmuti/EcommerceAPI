using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [Display(Name = "CartId")]
        public string UserId { get; set; }

        public User User { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int Count { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

        [NotMapped]
        public double Price { get; set; }

    }
}
