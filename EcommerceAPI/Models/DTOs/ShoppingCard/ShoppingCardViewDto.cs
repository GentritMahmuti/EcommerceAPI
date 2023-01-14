namespace EcommerceAPI.Models.DTOs.ShoppingCard
{
    public class ShoppingCardViewDto
    {
        public int ShoppingCardItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductImage { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int ShopingCardProductCount { get; set; }
        public double ProductPrice { get; set; }
        public double Total { get; set; }
    }
}
