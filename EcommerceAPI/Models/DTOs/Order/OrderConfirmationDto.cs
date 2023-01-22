namespace EcommerceAPI.Models.DTOs.Order
{
    public class OrderConfirmationDto
    {
        public string UserName { get; set; }
        public DateTime OrderDate { get; set; }
        public double Price { get; set; }
        public string OrderId { get; set; }
        public string Email { get; set; }
    }
}
