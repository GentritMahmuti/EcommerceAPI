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
        private readonly PaymentMethodService _paymentMethodService;
        private readonly ILogger<PaymentMethodEntity> _logger;
        private readonly PaymentIntentService _paymentIntentService;

        public StripeAppService(ChargeService chargeService, CustomerService customerService, TokenService tokenService, IUnitOfWork unitOfWork, PaymentMethodService paymentMethodService, ILogger<PaymentMethodEntity> logger, IOrderService orderService, PaymentIntentService paymentIntentService)
        {
            _chargeService = chargeService;
            _customerService = customerService;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _paymentMethodService = paymentMethodService;
            _logger = logger;
            _orderService = orderService;
            _paymentIntentService = paymentIntentService;
        }

        public async Task<StripeCustomer> AddStripeCustomerAsync(string userId, AddStripeCustomer customer, CancellationToken ct)
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
            Stripe.Token stripeToken = await _tokenService.CreateAsync(tokenOptions, null, ct);

            // Set Customer options using
            CustomerCreateOptions customerOptions = new CustomerCreateOptions
            {
                Name = customer.Name,
                Email = customer.Email,
                Source = stripeToken.Id
            };

            // Create customer at Stripe
            Customer createdCustomer = await _customerService.CreateAsync(customerOptions, null, ct);

            // Save Payment Method to the database
            var newPaymentMethod = new PaymentMethodEntity
            {
                PaymentMethodId = stripeToken.Card.Id,
                CustomerId = createdCustomer.Id,
                CardBrand = stripeToken.Card.Brand,
                CardLastFour = stripeToken.Card.Last4,
                ExpMonth = stripeToken.Card.ExpMonth,
                ExpYear = stripeToken.Card.ExpYear,
                UserId = userId
            };
            _unitOfWork.Repository<PaymentMethodEntity>().Create(newPaymentMethod);
            _unitOfWork.Complete();

            // Return the created customer at stripe
            return new StripeCustomer(createdCustomer.Name, createdCustomer.Email, createdCustomer.Id);
        }

        public async Task<string> AddStripePayment(string CustomerId, string PaymentId, string orderId)
        {
            var orderData = _unitOfWork.Repository<OrderData>().GetByCondition(o => o.OrderId == orderId).FirstOrDefault();

            //choose one of payment methods
            var paymentMethod = _unitOfWork.Repository<PaymentMethodEntity>().GetByCondition(p => p.PaymentMethodId == PaymentId).FirstOrDefault();

            //create payment intent
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(orderData.OrderFinalPrice * 100),
                Currency = "usd",
                Customer = CustomerId,
                PaymentMethod = PaymentId,
                Confirm = true,
                OffSession = true,
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
            };

            var createdPayment = await _paymentIntentService.CreateAsync(options);

            if (createdPayment.Status == "succeeded")
            {
                orderData.PaymentStatus = "paid";
                orderData.TransactionId = createdPayment.Id;
                orderData.PaymentDate = DateTime.Now;
                orderData.PaymentDueDate = DateTime.Now.AddDays(30);
                orderData.PaymentMethodId = paymentMethod.PaymentMethodId;

                _unitOfWork.Complete();

                return "Payment was successful!";
            }
            else
            {
                return "Payment failed. Please try again.";
            }

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
                orderData.PaymentMethodId = createdPayment.PaymentMethod;

                _unitOfWork.Complete();

                return "Payment was successful!";
            }
            else
            {
                return "Payment failed. Please try again.";
            }
        }


        public List<PaymentMethodEntity> GetPaymentMethodsByCustomer(string userId)
        {
            return _unitOfWork.Repository<PaymentMethodEntity>().GetByCondition(p => p.UserId == userId).ToList();
        }

        public async Task DeletePaymentMethod(string paymentMethodId)
        {
            // Delete the payment method from the database
            var paymentMethod = _unitOfWork.Repository<PaymentMethodEntity>().GetByCondition(p => p.PaymentMethodId == paymentMethodId).FirstOrDefault();
            _unitOfWork.Repository<PaymentMethodEntity>().Delete(paymentMethod);
            _unitOfWork.Complete();

            // Delete the payment method from the Stripe API
            await _paymentMethodService.DetachAsync(paymentMethodId);
        }

        public async Task UpdatePaymentMethodExpiration(string paymentMethodId, int expYear, int expMonth)
        {
            // Update the expiration year and month in the database
            var paymentMethod = _unitOfWork.Repository<PaymentMethodEntity>().GetByCondition(p => p.PaymentMethodId == paymentMethodId).FirstOrDefault();
            paymentMethod.ExpYear = expYear;
            paymentMethod.ExpMonth = expMonth;
            _unitOfWork.Complete();

            // Update the expiration year and month in the Stripe API
            var updateOptions = new PaymentMethodUpdateOptions
            {
                Card = new PaymentMethodCardOptions
                {
                    ExpYear = expYear,
                    ExpMonth = expMonth
                }
            };

            await _paymentMethodService.UpdateAsync(paymentMethodId, updateOptions);
        }

        public async Task<PaymentMethodEntity> CreatePaymentMethod(string userId, string cardNumber, string expMonth, string expYear, string cvc)
        {
            // Create the payment method in the Stripe API using the token
            var paymentMethodOptions = new PaymentMethodCreateOptions
            {
                Type = "card",
                Card = new PaymentMethodCardOptions
                {
                    Number = cardNumber,
                    ExpMonth = (long)Convert.ToDouble(expMonth),
                    ExpYear = (long)Convert.ToDouble(expYear),
                    Cvc = cvc
                }
            };

            var paymentMethodService = new PaymentMethodService();
            var paymentMethod = await paymentMethodService.CreateAsync(paymentMethodOptions);


            // Save the payment method to the database
            var paymentMethodEntity = new PaymentMethodEntity
            {
                PaymentMethodId = paymentMethod.Id,
                CardBrand = paymentMethod.Card.Brand,
                CardLastFour = paymentMethod.Card.Last4,
                ExpMonth = paymentMethod.Card.ExpMonth,
                ExpYear = paymentMethod.Card.ExpYear,
                UserId = userId
            };

            _unitOfWork.Repository<PaymentMethodEntity>().Create(paymentMethodEntity);
            _unitOfWork.Complete();
            _logger.LogInformation("Payment method created successfully");

            return paymentMethodEntity;
        }

        public async Task AttachPaymentMethodToCustomer(string customerId, string paymentMethodId)
        {
            var paymentMethodAttachOptions = new PaymentMethodAttachOptions
            {
                Customer = customerId
            };

            var paymentMethod = _unitOfWork.Repository<PaymentMethodEntity>().GetByCondition(p => p.PaymentMethodId == paymentMethodId).FirstOrDefault();
            paymentMethod.CustomerId = customerId;
            _unitOfWork.Complete();

            await _paymentMethodService.AttachAsync(paymentMethodId, paymentMethodAttachOptions);
        }
    }
}
