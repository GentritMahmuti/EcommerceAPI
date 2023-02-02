using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.ShoppingCard;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IConfiguration _configuration;
        private readonly IValidator<AddressDetails> _addressDetailsValidator;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, IConfiguration configuration, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _configuration = configuration;
            _logger = logger;
        }

        [Authorize(Roles = "LifeAdmin")]
        [HttpPost("ChangeOrderStatus")]
        public async Task<IActionResult> ChangeOrderStatus(string orderId, string status)
        {
            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(status))
            {
                return BadRequest("Invalid order id or status");
            }
            try
            {
                await _orderService.ChangeOrderStatus(orderId, status);
                return Ok("Order status changed!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "LifeUser")]
        [HttpGet("GetCustomerOrderHistory")]
        public async Task<IActionResult> GetCustomerOrderHistory()
        {
            try
            {
                var userData = (ClaimsIdentity)User.Identity;
                var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (userId == null) { return Unauthorized(); }

                List<OrderData> orderHistory = _orderService.GetCustomerOrderHistory(userId);

                return Ok(orderHistory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "LifeUser, LifeAdmin")]
        [HttpPost("ProductSummaryForOrder")]
        public async Task<IActionResult> ProductSummary(ProductSummaryModel model)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            if (claimsIdentity == null || !claimsIdentity.IsAuthenticated)
            {
                return Unauthorized();
            }

            try
            {
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                model.AddressDetails.Email = claimsIdentity.FindFirst(ClaimTypes.Email).Value;

                await _orderService.CreateOrder(userId, model.AddressDetails, model.PromoCode);

                return Ok();
            }       
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "LifeUser, LifeAdmin")]
        [HttpPost("CreateOrderForProduct")]
        public async Task<IActionResult> CreateOrderForProduct(int productId, int count, AddressDetails addressDetails, string? promoCode)
        {
            try
            {
                // await _addressDetailsValidator.ValidateAndThrowAsync(addressDetails);
                var userData = (ClaimsIdentity)User.Identity;
                var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (userId == null) { return Unauthorized(); }

                await _orderService.CreateOrderForProduct(userId, productId, count, addressDetails, promoCode);

                return Ok("Order created!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)} - Error while creating order for product!");
                return BadRequest($"An error happened: '{ex.Message}'");
            }
        }
    }
}
