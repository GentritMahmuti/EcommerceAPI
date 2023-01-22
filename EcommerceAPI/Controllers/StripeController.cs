using EcommerceAPI.Models.DTOs.Stripe;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        private readonly IStripeAppService _stripeService;

        public StripeController(IStripeAppService stripeService)
        {
            _stripeService = stripeService;
        }

        [HttpPost("customer/add")]
        public async Task<ActionResult<StripeCustomer>> AddStripeCustomer([FromBody] AddStripeCustomer customer, CancellationToken ct)
        {
            StripeCustomer createdCustomer = await _stripeService.AddStripeCustomerAsync(customer, ct);

            return StatusCode(StatusCodes.Status200OK, createdCustomer);
        }

        [HttpPost("payment/add")]
        public async Task<ActionResult<string>> AddStripePayment([FromBody] AddStripePayment payment, CancellationToken ct, string orderId)
        {
            string paymentId = await _stripeService.AddStripePaymentAsync(payment, ct, orderId);

            return StatusCode(StatusCodes.Status200OK, paymentId);
        }

    }
}
