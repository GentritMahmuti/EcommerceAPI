using EcommerceAPI.Models.DTOs.Stripe;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<string>> AddStripePayment([FromBody] AddStripePayment payment, CancellationToken ct, string orderId)
        {
            string paymentId = await _stripeService.AddStripePaymentAsync(payment, ct, orderId);

            return StatusCode(StatusCodes.Status200OK, paymentId);
        }

        [HttpGet("payment/methods")]
        public ActionResult<List<PaymentMethodEntity>> GetPaymentMethodsByCustomer(string customerId)
        {
            List<PaymentMethodEntity> paymentMethods = _stripeService.GetPaymentMethodsByCustomer(customerId);

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
