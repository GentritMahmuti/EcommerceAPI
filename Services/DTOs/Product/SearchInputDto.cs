namespace Services.DTOs.Product
{
    public class SearchInputDto
    {
        public string Title { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public string? SortByPopularity { get; set; } = null; 
        public string? SortByPrice { get; set; } = null; 
    }
}
