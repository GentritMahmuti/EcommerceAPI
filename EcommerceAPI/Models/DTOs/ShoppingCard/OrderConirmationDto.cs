namespace EcommerceAPI.Models.DTOs.ShoppingCard
{
    public class OrderConirmationDto
    {
        public string UserName { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public double Price { get; set; }
        public string OrderId { get; set; }
    }
}
