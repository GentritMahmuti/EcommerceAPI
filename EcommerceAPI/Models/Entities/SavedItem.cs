using Nest;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.Entities
{
    public class SavedItem
    {
        [Key]
        public string SavedItemId { get; set; }

        [Display(Name = "SavedItemId")]
        public string UserId { get; set; }

        public User User { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

    }
}
