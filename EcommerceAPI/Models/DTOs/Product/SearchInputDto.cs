namespace EcommerceAPI.Models.DTOs.Product
{
    public class SearchInputDto
    {
        public string Title { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
    }
}
