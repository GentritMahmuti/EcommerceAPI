using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.Entities
{
    public class Book
    {
        [Required]
        public int Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Range(1, 10000)]
        public double ListPrice { get; set; }
    }
}
