using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Models.DTOs.Stripe;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using StackExchange.Redis;
using Stripe;

namespace EcommerceAPI.Services
{
    public class StripeAppService : IStripeAppService
    {
        private readonly ChargeService _chargeService;
        private readonly CustomerService _customerService;
        private readonly TokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;

        public StripeAppService(ChargeService chargeService, CustomerService customerService, TokenService tokenService, IUnitOfWork unitOfWork)
        {
            _chargeService = chargeService;
            _customerService = customerService;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<StripeCustomer> AddStripeCustomerAsync(AddStripeCustomer customer, CancellationToken ct)
        {
            // Set Stripe Token options based on customer data
            TokenCreateOptions tokenOptions = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Number = customer.CreditCard.CardNumber,
                    ExpYear = customer.CreditCard.ExpirationYear,
                    ExpMonth = customer.CreditCard.ExpirationMonth,
                    Cvc = customer.CreditCard.Cvc
                }
            };

            // Create new Stripe Token
            Token stripeToken = await _tokenService.CreateAsync(tokenOptions, null, ct);

            // Set Customer options using
            CustomerCreateOptions customerOptions = new CustomerCreateOptions
            {
                Name = customer.Name,
                Email = customer.Email,
                Source = stripeToken.Id
            };

            // Create customer at Stripe
            Customer createdCustomer = await _customerService.CreateAsync(customerOptions, null, ct);

            // Return the created customer at stripe
            return new StripeCustomer(createdCustomer.Name, createdCustomer.Email, createdCustomer.Id);
        }

      
        public async Task<string> AddStripePaymentAsync(AddStripePayment payment, CancellationToken ct, string orderId)
        {
            var orderData = _unitOfWork.Repository<OrderData>().GetByCondition(o => o.OrderId == orderId).FirstOrDefault();

            ChargeCreateOptions paymentOptions = new ChargeCreateOptions
            {
                Customer = payment.CustomerId,
                ReceiptEmail = payment.ReceiptEmail,
                Description = "Order Payment",
                Currency = "usd",
                Amount = (long)(orderData.OrderFinalPrice * 100)
            };

            var createdPayment = await _chargeService.CreateAsync(paymentOptions, null, ct);



            if (createdPayment.Status == "succeeded")
            {
                orderData.PaymentStatus = "paid";
                orderData.TransactionId = createdPayment.Id;
                orderData.PaymentDate = DateTime.Now;
                orderData.PaymentDueDate = DateTime.Now.AddDays(30);

                _unitOfWork.Complete();
                
                return "Payment was successful!";
            }
            else
            {
                return "Payment failed. Please try again.";
            }
        }
    }
}
