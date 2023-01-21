using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }

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

        public Category Category { get; set; }

        public int Stock { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

        public ICollection<Review> SubmittedReviews { get; set; }

        public ICollection<ProductOrderData> ProductOrderData { get; set; }

    }
}
