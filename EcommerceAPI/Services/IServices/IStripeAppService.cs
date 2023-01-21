using EcommerceAPI.Models.DTOs.Stripe;

namespace EcommerceAPI.Services.IServices
{
    public interface IStripeAppService
    {
        Task<StripeCustomer> AddStripeCustomerAsync(AddStripeCustomer customer, CancellationToken ct);
        Task<StripePayment> AddStripePaymentAsync(AddStripePayment payment, CancellationToken ct, string orderId);
    }
    
}
