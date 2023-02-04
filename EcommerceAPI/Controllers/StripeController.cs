using Domain.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Stripe;
using Services.Services.IServices;
using Stripe;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StripeController : ControllerBase
    {
        private readonly IStripeAppService _stripeService;
        private readonly PaymentMethodService _paymentMethodService;

        public StripeController(IStripeAppService stripeService, PaymentMethodService paymentMethodService)
        {
            _stripeService = stripeService;
            _paymentMethodService = paymentMethodService;
        }

        [HttpPost("customer/add")]
        public async Task<ActionResult<StripeCustomer>> AddStripeCustomer([FromBody] AddStripeCustomer customer, CancellationToken ct)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            StripeCustomer createdCustomer = await _stripeService.AddStripeCustomerAsync(userId, customer, ct);

            return StatusCode(StatusCodes.Status200OK, createdCustomer);
        }

        [HttpPost("payment/add")]
        public async Task<ActionResult<string>> AddStripePayment(string CustomerId, string PaymentId, string orderId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            string paymentId = await _stripeService.AddStripePayment(CustomerId, PaymentId, orderId);

            return StatusCode(StatusCodes.Status200OK, paymentId);
        }

        [HttpPost("payment/method/add")]
        public async Task<ActionResult<PaymentMethodEntity>> AddPaymentMethod(string cardNumber, string expMonth, string expYear, string cvc)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            PaymentMethodEntity paymentMethod = await _stripeService.CreatePaymentMethod(userId, cardNumber, expMonth, expYear, cvc);

            return StatusCode(StatusCodes.Status200OK, paymentMethod);
        }

        [HttpPost("payment/method/attach")]
        public async Task<ActionResult> AttachPaymentMethodToCustomer(string customerId, string paymentMethodId)
        {
            await _stripeService.AttachPaymentMethodToCustomer(customerId, paymentMethodId);

            return StatusCode(StatusCodes.Status200OK);
        }


        //[HttpGet("payment/methods")]
        //public ActionResult<List<PaymentMethodEntity>> GetPaymentMethodsByCustomer()
        //{
        //    var userData = (ClaimsIdentity)User.Identity;
        //    var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

        //    if (userId == null) { return Unauthorized(); }

        //    List<PaymentMethodEntity> paymentMethods = _stripeService.GetPaymentMethodsByCustomer(userId);

        //    return StatusCode(StatusCodes.Status200OK, paymentMethods);
        //}
        [HttpGet("payment/methods")]
        public ActionResult<List<PaymentMethodEntity>> GetPaymentMethodsByCustomer()
        {
            var userData = (ClaimsIdentity)User.Identity;
            if (userData == null) { return Unauthorized(); }

            var userId = userData.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) { return Unauthorized(); }

            List<PaymentMethodEntity> paymentMethods = _stripeService.GetPaymentMethodsByCustomer(userId);

            return StatusCode(StatusCodes.Status200OK, paymentMethods);
        }


        [HttpDelete("payment/method/delete")]
        public async Task DeletePaymentMethod(string paymentMethodId)
        {
            await _stripeService.DeletePaymentMethod(paymentMethodId);
        }

        [HttpPut("payment/method/update")]
        public async Task UpdatePaymentMethodExpiration(string paymentMethodId, int expYear, int expMonth)
        {
            await _stripeService.UpdatePaymentMethodExpiration(paymentMethodId, expYear, expMonth);
        }
    }
}
