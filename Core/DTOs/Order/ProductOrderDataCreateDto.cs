using Domain.Entities;
using DurableTask.Core.Common;

namespace Services.DTOs.Order
{
    public class ProductOrderDataCreateDto
    {
        public int ProductId { get; set; }
        public Domain.Entities.Product Product { get; set; }
        public string OrderDataId { get; set; }
        public OrderData OrderData { get; set; }

        public int Count { get; set; }

        public double Price { get; set; }
    }
}
