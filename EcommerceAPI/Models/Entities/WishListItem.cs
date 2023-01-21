using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.Entities
{
    public class WishListItem
    {
        [Key]
        public string WishListItemId { get; set; }

        [Display(Name = "WishListId")]
        public string UserId { get; set; }

        public User User { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

    }
}
