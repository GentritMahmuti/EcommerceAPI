using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Models.DTOs.Order
{
    public class ProductOrderDataDto
    {
        public int ProductId { get; set; }
        public Entities.Product Product { get; set; }
        public string OrderDataId { get; set; }
        public OrderData OrderData { get; set; }

        public int Count { get; set; }

        public double Price { get; set; }
    }
}
