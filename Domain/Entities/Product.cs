using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }


        [Display(Name = "Product Description"), DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public double ListPrice { get; set; }

        [Display(Name = "Price")]
        public double Price { get; set; }

        public string ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public int Stock { get; set; }
        public int TotalSold { get; set; } = 0;
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

        public ICollection<Review> SubmittedReviews { get; set; }

        public ICollection<ProductOrderData> ProductOrderData { get; set; }

    }
}
