namespace EcommerceAPI.Models.DTOs.Product
{
    public class ProductFilter
    {
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public int? CategoryId { get; set; }
    }
}
