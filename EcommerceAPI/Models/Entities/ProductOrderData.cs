namespace EcommerceAPI.Models.Entities
{
    public class ProductOrderData
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string OrderDataId { get; set; }
        public OrderData OrderData { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}
