

using Domain.Entities;
using Services.DTOs.Stripe;

namespace Services.Services.IServices
{
    public interface IStripeAppService
    {
        Task<StripeCustomer> AddStripeCustomerAsync(string userId, AddStripeCustomer customer, CancellationToken ct);
        Task<string> AddStripePaymentAsync(AddStripePayment payment, CancellationToken ct, string orderId);
        Task<string> AddStripePayment(string CustomerId, string PaymentId, string orderId);
        List<PaymentMethodEntity> GetPaymentMethodsByCustomer(string userId);
        Task DeletePaymentMethod(string paymentMethodId);
        Task UpdatePaymentMethodExpiration(string paymentMethodId, int expYear, int expMonth);
        Task AttachPaymentMethodToCustomer(string customerId, string paymentMethodId);
        Task<PaymentMethodEntity> CreatePaymentMethod(string userId, string cardNumber, string expMonth, string expYear, string cvc);
    }
    
}
