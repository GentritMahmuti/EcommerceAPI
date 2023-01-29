namespace ECommerce.Consumer.Entities
{
    public class OrderConfirmationDto
    {
        public string UserName { get; set; }
        public DateTime OrderDate { get; set; }
        public double Price { get; set; }
        public string OrderId { get; set; }
        public string Email { get; set; }
        public string StreetAddress { get; set; }
        public string PhoheNumber { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

    }
}
