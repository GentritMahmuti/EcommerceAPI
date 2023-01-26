using EcommerceAPI.Models.DTOs.Stripe;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface IStripeAppService
    {
        Task<StripeCustomer> AddStripeCustomerAsync(string userId, AddStripeCustomer customer, CancellationToken ct);
        Task<string> AddStripePaymentAsync(AddStripePayment payment, CancellationToken ct, string orderId);
        List<PaymentMethodEntity> GetPaymentMethodsByCustomer(string customerId);
        Task DeletePaymentMethod(string paymentMethodId);
        Task UpdatePaymentMethodExpiration(string paymentMethodId, int expYear, int expMonth);
    }
    
}
