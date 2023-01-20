namespace EcommerceAPI.Models.DTOs.Stripe
{
    public class StripeCustomer
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string CustomerId { get; set; }

        public StripeCustomer(string name, string email, string customerId)
        {
            Name = name;
            Email = email;
            CustomerId = customerId;
        }
    }
}
