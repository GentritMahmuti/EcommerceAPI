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

        /// <summary>
        /// Adds a Stripe customer for the authenticated user.
        /// </summary>
        /// <param name="customer">The customer details to be added to Stripe</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>The created Stripe customer</returns>
        [HttpPost("AddCustomer")]
        public async Task<ActionResult<StripeCustomer>> AddStripeCustomer([FromBody] AddStripeCustomer customer, CancellationToken ct)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            StripeCustomer createdCustomer = await _stripeService.AddStripeCustomerAsync(userId, customer, ct);

            return StatusCode(StatusCodes.Status200OK, createdCustomer);
        }

        /// <summary>
        /// Add a new payment for a customer.
        /// </summary>
        /// <param name="CustomerId">The customer id associated with the payment</param>
        /// <param name="PaymentId">The payment id</param>
        /// <param name="orderId">The order id associated with the payment</param>
        /// <returns>The payment id</returns>
        [HttpPost("CreatePaymentForOrder")]
        public async Task<ActionResult<string>> AddStripePayment(string CustomerId, string PaymentId, string orderId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            string paymentId = await _stripeService.AddStripePayment(CustomerId, PaymentId, orderId);

            return StatusCode(StatusCodes.Status200OK, paymentId);
        }


        /// <summary>
        /// Adds a payment method to the user's account
        /// </summary>
        /// <param name="cardNumber">The card number of the payment method</param>
        /// <param name="expMonth">The expiration month of the payment method</param>
        /// <param name="expYear">The expiration year of the payment method</param>
        /// <param name="cvc">The CVC code of the payment method</param>
        /// <returns>The created payment method</returns>
        [HttpPost("AddPaymentMethod")]
        public async Task<ActionResult<PaymentMethodEntity>> AddPaymentMethod(string cardNumber, string expMonth, string expYear, string cvc)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            PaymentMethodEntity paymentMethod = await _stripeService.CreatePaymentMethod(userId, cardNumber, expMonth, expYear, cvc);

            return StatusCode(StatusCodes.Status200OK, paymentMethod);
        }

        /// <summary>
        /// Adds a payment method to the user's account
        /// </summary>
        /// <param name="cardNumber">The card number of the payment method</param>
        /// <param name="expMonth">The expiration month of the payment method</param>
        /// <param name="expYear">The expiration year of the payment method</param>
        /// <param name="cvc">The cvc of the payment method</param>
        /// <returns>A payment method entity</returns>
        [HttpPost("AttachPaymentMethod")]
        public async Task<ActionResult> AttachPaymentMethodToCustomer(string customerId, string paymentMethodId)
        {
            await _stripeService.AttachPaymentMethodToCustomer(customerId, paymentMethodId);

            return StatusCode(StatusCodes.Status200OK);
        }


        /// <summary>
        /// Retrieves the payment methods for the current user.
        /// </summary>
        /// <returns>A list of <see cref="PaymentMethodEntity"/> that belong to the current user.</returns>
        /// <response code="200">The payment methods were successfully retrieved.</response>
        /// <response code="401">The user is unauthorized to access this resource.</response>
        [HttpGet("GetPaymentMethods")]
        public ActionResult<List<PaymentMethodEntity>> GetPaymentMethodsByCustomer()
        {
            var userData = (ClaimsIdentity)User.Identity;
            if (userData == null) { return Unauthorized(); }

            var userId = userData.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) { return Unauthorized(); }

            List<PaymentMethodEntity> paymentMethods = _stripeService.GetPaymentMethodsByCustomer(userId);

            return StatusCode(StatusCodes.Status200OK, paymentMethods);
        }


        /// <summary>
        /// Deletes a payment method associated with a customer.
        /// </summary>
        /// <param name="paymentMethodId">The identifier of the payment method to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [HttpDelete("DeletePaymentMethod")]
        public async Task DeletePaymentMethod(string paymentMethodId)
        {
            await _stripeService.DeletePaymentMethod(paymentMethodId);
        }

        /// <summary>
        /// Updates the expiration date for a payment method.
        /// </summary>
        /// <param name="paymentMethodId">The ID of the payment method to update</param>
        /// <param name="expYear">The new expiration year of the payment method</param>
        /// <param name="expMonth">The new expiration month of the payment method</param>
        /// <returns></returns>
        [HttpPut("UpdatePaymentMethod")]
        public async Task UpdatePaymentMethodExpiration(string paymentMethodId, int expYear, int expMonth)
        {
            await _stripeService.UpdatePaymentMethodExpiration(paymentMethodId, expYear, expMonth);
        }
    }
}
