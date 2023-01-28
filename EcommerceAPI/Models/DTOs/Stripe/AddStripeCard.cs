namespace EcommerceAPI.Models.DTOs.Stripe
{
    public class AddStripeCard
    {
        public string CardNumber { get; set; }
        public string ExpirationYear { get; set; }
        public string ExpirationMonth { get; set; }
        public string Cvc { get; set; }
    }
}
