using EcommerceAPI.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models.DTOs.Order
{
    public class OrderDetailsCreateDto
    {
        public string OrderId { get; set; }
        [ForeignKey("OrderId")]
        public OrderData OrderData { get; set; }
        public int ProductId { get; set; }
        public Entities.Product Product { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}
