using EcommerceAPI.Models.DTOs.OrderDetails;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace EcommerceAPI.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly IOrderDetailsService _orderDetailsService;
        private readonly IConfiguration _configuration;

        public OrderDetailsController(IOrderDetailsService orderDetailsService, IConfiguration configuration)
        {
            _orderDetailsService = orderDetailsService;
            _configuration = configuration;
        }

        [HttpGet("GetOrderDetails")]
        public async Task<IActionResult> Get(int id)
        {
            var orderDetails = await _orderDetailsService.GetOrderDetails(id);

            if (orderDetails == null)
            {
                return NotFound();
            }

            return Ok(orderDetails);
        }


        [HttpGet("GetAllOrderDetails")]
        public async Task<IActionResult> GetAllOrderDetails()
        {
            var orderDetails = await _orderDetailsService.GetAllOrderDetails();

            return Ok(orderDetails);
        }

       

        [HttpPost("PostOrderDetails")]
        public async Task<IActionResult> Post(OrderDetailsCreateDto OrderDetailsToCreate)
        {
            await _orderDetailsService.CreateOrderDetails(OrderDetailsToCreate);

            return Ok("OrderDetails created successfully!");
        }

        [HttpPut("UpdateOrderDetails")]
        public async Task<IActionResult> Update(OrderDetails OrderDetailsToUpdate)
        {
            await _orderDetailsService.UpdateOrderDetails(OrderDetailsToUpdate);

            return Ok("OrderDetails updated successfully!");
        }

        [HttpDelete("DeleteOrderDetails")]
        public async Task<IActionResult> Delete(int id)
        {
            await _orderDetailsService.DeleteOrderDetails(id);

            return Ok("OrderDetails deleted successfully!");
        }
    }
}
