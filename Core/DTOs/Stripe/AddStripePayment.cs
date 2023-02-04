namespace Services.DTOs.Stripe
{
    public class AddStripePayment
    {
        public string CustomerId { get; set; }
        public string ReceiptEmail { get; set; }
        public string PaymentId { get; set; }
    }
}
