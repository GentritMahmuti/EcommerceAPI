using EcommerceAPI.Models.Entities;
using System.ComponentModel.DataAnnotations;
namespace EcommerceAPI.Models.DTOs.Product
{
    public class ProductCreateDto
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Range(1, 10000)]
        public double ListPrice { get; set; }
        [Required]
        [Range(1, 10000)]
        public double Price { get; set; }
        [Required]
        [Range(1, 10000)]
        public double Price50 { get; set; }
        [Required]
        [Range(1, 10000)]
        public double Price100 { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int CoverTypeId { get; set; }
        public CoverType CoverType { get; set; }
    }
}
