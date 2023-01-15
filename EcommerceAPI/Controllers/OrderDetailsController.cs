using EcommerceAPI.Data;
using EcommerceAPI.Models.DTOs.OrderDetails;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services;
using EcommerceAPI.Services.IServices;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using Nest;
using StackExchange.Redis;
using System.Collections.Generic;

namespace EcommerceAPI.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly IOrderDetailsService _orderDetailsService;
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cacheService;
        private readonly IValidator<OrderDetails> _orderDetailsValidator;

        public OrderDetailsController(IOrderDetailsService orderDetailsService, IConfiguration configuration, ICacheService cacheService, IValidator<OrderDetails> orderDetailsValidator)
        {
            _orderDetailsService = orderDetailsService;
            _configuration = configuration;
            _cacheService = cacheService;
            _orderDetailsValidator = orderDetailsValidator;
        }


        [HttpGet("GetOrderDetails/{id}")]

        public async Task<IActionResult> Get(int id)
        {
            var cacheData = _cacheService.GetData<OrderDetails>($"orderData-{id}");
            if (cacheData != null)
            {
                return Ok(cacheData);
            }
            else
            {
                var data = await _orderDetailsService.GetOrderDetails(id);
                if (data == null)
                {
                    return NotFound();
                }
                var expiryTime = DateTimeOffset.Now.AddMinutes(5);
                _cacheService.SetData<OrderDetails>($"orderData-{id}", data, expiryTime);
                return Ok(data);
            }
        }

        [HttpGet("GetAllOrderDetails")]
        public async Task<IActionResult> GetOrderDetails()
        {
            var cacheData = _cacheService.GetData<List<OrderDetails>>("orderData");
            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(cacheData);
            }
            else
            {
                var data = await _orderDetailsService.GetAllOrderDetails();
                var expiryTime = DateTimeOffset.Now.AddMinutes(5);
                _cacheService.SetData<List<OrderDetails>>("orderData", data, expiryTime);
                return Ok(data);
            }
        }


        [HttpPost("PostOrderDetails")]
        public async Task<IActionResult> Post(OrderDetailsCreateDto OrderDetailsToCreate)
        {
            var orderDetails = await _orderDetailsService.CreateOrderDetails(OrderDetailsToCreate);
            var expiryTime = DateTimeOffset.Now.AddMinutes(5);
            var key = $"orderData-{orderDetails.Id}";
            _cacheService.SetData<OrderDetails>(key, orderDetails, expiryTime);
            return Ok("OrderDetails created successfully!");
        }


        [HttpPut("UpdateOrderDetails")]
        public async Task<IActionResult> Update(OrderDetails OrderDetailsToUpdate)
        {
            var key = "orderDetails_" + OrderDetailsToUpdate.Id;
            var cacheData = _cacheService.GetUpdatedData<OrderDetails>(key);
            if (cacheData == null)
            {
                var orderDetails = await _orderDetailsService.GetOrderDetails(OrderDetailsToUpdate.Id);
                if (orderDetails == null)
                {
                    return NotFound("OrderDetails not found!");
                }
                cacheData = orderDetails;
            }
            cacheData.Id = OrderDetailsToUpdate.Id;
            cacheData.OrderData = OrderDetailsToUpdate.OrderData;
            cacheData.ProductId = OrderDetailsToUpdate.ProductId;
            cacheData.Count = OrderDetailsToUpdate.Count;
            cacheData.Price = OrderDetailsToUpdate.Price;
            await _orderDetailsService.UpdateOrderDetails(cacheData);
            var expiryTime = DateTimeOffset.Now.AddMinutes(5);
            _cacheService.SetUpdatedData(key, cacheData, expiryTime);
            return Ok("OrderDetails updated successfully!");
        }

        [HttpDelete("DeleteOrderDetails")]
        public async Task<IActionResult> Delete(int id)
        {
            var key = "orderDetails_" + id;
            var cacheData = _cacheService.GetData<OrderDetails>(key);
            if (cacheData != null)
            {
                _cacheService.RemoveData(key);
            }
            var orderDetails = await _orderDetailsService.GetOrderDetails(id);
            if (orderDetails == null)
            {
                return NotFound("OrderDetails not found!");
            }
            await _orderDetailsService.DeleteOrderDetails(id);
            return Ok("OrderDetails deleted successfully!");
        }

    }
}
