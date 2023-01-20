using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("ProcessOrder")]
        public async Task<IActionResult> ProcessOrder(List<string> orderIds, string status)
        {
            await _orderService.ProcessOrder(orderIds, status);

            return Ok($"Now selected orders are in new status: {status}");
        }
    }
}
