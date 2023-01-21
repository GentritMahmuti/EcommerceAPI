using EcommerceAPI.Models.Entities;
using System.ComponentModel.DataAnnotations;
namespace EcommerceAPI.Models.DTOs.Product
{
    public class ProductCreateDto
    {
        [Required, StringLength(100), Display(Name = "Name")]
        public string Name { get; set; }

        [Required, StringLength(10000), Display(Name = "Product Description"), DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public double ListPrice { get; set; }

        [Display(Name = "Price")]
        public double Price { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public int Stock { get; set; }

        //public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
