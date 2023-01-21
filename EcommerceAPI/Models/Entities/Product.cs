using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }


        [Display(Name = "Product Description")]
        public string Description { get; set; }

        public double ListPrice { get; set; }

        public double Price { get; set; }

        public string ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public int Stock { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<Review> SubmittedReviews { get; set; }

        public ICollection<ProductOrderData> ProductOrderData { get; set; }

    }
}
