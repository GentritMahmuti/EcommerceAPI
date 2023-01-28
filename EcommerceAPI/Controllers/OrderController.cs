using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
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

        public OrderController(IOrderService orderService, IConfiguration configuration)
        {
            _orderService = orderService;
            _configuration = configuration;
        }

        //[Authorize(Roles = "LifeAdmin")]
        [HttpPost("ProcessOrder")]
        public async Task<IActionResult> ProcessOrder(string orderId, string status)
        {
            await _orderService.ProcessOrder(orderId, status);
            return Ok($"Now selected order is in new status: {status}");
        }

        [Authorize(Roles = "LifeUser")]
        [HttpGet("GetOrderHistory")]
        public async Task<IActionResult> GetOrderHistory()
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            List<OrderData> orderHistory = _orderService.GetOrderHistory(userId);

            return Ok(orderHistory);
        }
    }
}
