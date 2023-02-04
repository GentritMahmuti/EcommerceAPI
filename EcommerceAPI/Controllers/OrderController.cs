using Domain.Entities;
using EcommerceAPI.Services.IServices;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Order;
using Services.DTOs.ShoppingCard;
using Services.Services.IServices;
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

        /// <summary>
        /// Gets a specific order by id.
        /// </summary>
        /// <param name="orderId">The id of the order to retrieve</param>
        /// <returns>The order with the specified id</returns>
        /// <response code="200">Returns the order with the specified id</response>
        /// <response code="400">If there was an error retrieving the order</response>
        /// <tags>Order</tags>
        [Authorize(Roles = "LifeAdmin")]
        [HttpGet("GetOrder")]
        public async Task<IActionResult> GetOrder(string orderId)
        {
            try
            {
                var order = await _orderService.GetOrder(orderId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(OrderController)} - Error when getting order by id.");
                return BadRequest("An error happened: " + ex.Message);
            }
        }

        /// <summary>
        /// Changes the status of an order with the given id and sends a message to a rabbit queue, which then sends an email.
        /// </summary>
        /// <param name="orderId">The id of the order to change the status of</param>
        /// <param name="status">The new status to set for the order</param>
        /// <returns>A message indicating that the order status has been changed successfully</returns>
        /// <response code="200">Returns a message indicating that the order status has been changed successfully</response>
        /// <response code="400">If either the `orderId` or `status` parameter is empty or null</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Order</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" role to access.
        /// </remarks>
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
                _logger.LogError(ex, $"{nameof(OrderController)} - Error when changing order status.");
                return BadRequest("An error happened: " + ex.Message);
            }
        }


        /// <summary>
        /// Gets order history for a customer.
        /// </summary>
        /// <returns>The order history for the customer</returns>
        /// <response code="200">Returns the order history for the customer</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" or "LifeUser" role to access.
        /// </remarks>
        [Authorize]
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
                _logger.LogError(ex, $"{nameof(OrderController)} - Error when getting customer order history!");
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Creates order with products that are in shoppingCard.
        /// </summary>
        /// <param name="model">The product summary and address information to be used for the order</param>
        /// <returns></returns>
        /// <response code="200">Order was successfully created</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <response code="400">If there was an error creating the order</response>
        /// <remarks>
        /// This action requires authentication and the "LifeUser" or "LifeAdmin" role to access.
        /// </remarks>
        [Authorize]
        [HttpPost("CreateOrderFromShoppingCard")]
        public async Task<IActionResult> CreateOrderFromShoppingCard(ProductSummaryModel model)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            if (claimsIdentity == null || !claimsIdentity.IsAuthenticated)
            {
                return Unauthorized();
            }

            try
            {
               // await _addressDetailsValidator.ValidateAndThrowAsync(model.AddressDetails);
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                model.AddressDetails.Email = claimsIdentity.FindFirst(ClaimTypes.Email).Value;

                await _orderService.CreateOrder(userId, model.AddressDetails, model.PromoCode);

                return Ok("Order created successfully!");
            }       
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(OrderController)} - Error when creating order from shoppingCard");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Creates an order for a single product.
        /// </summary>
        /// <param name="productId">The id of the product to be ordered</param>
        /// <param name="count">The quantity of the product to be ordered</param>
        /// <param name="addressDetails">The details of the address where the product will be delivered</param>
        /// <param name="promoCode">The optional promo code to be used for the order</param>
        /// <returns>200 OK response with message "Order created!" if successful, otherwise a BadRequest with an error message</returns>
        /// <response code="200">Order created successfully</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <response code="400">If the request is invalid or an error occurs while processing the order</response>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" or "LifeUser" role to access.
        /// </remarks>
        [Authorize]
        [HttpPost("CreateOrderForProduct")]
        public async Task<IActionResult> CreateOrderForProduct(int productId, int count, AddressDetails addressDetails, string? promoCode)
        {
            try
            {
             //   await _addressDetailsValidator.ValidateAndThrowAsync(addressDetails);
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
