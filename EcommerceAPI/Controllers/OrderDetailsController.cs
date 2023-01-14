﻿using EcommerceAPI.Data;
using EcommerceAPI.Models.DTOs.OrderDetails;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services;
using EcommerceAPI.Services.IServices;
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

        public OrderDetailsController(IOrderDetailsService orderDetailsService, IConfiguration configuration, ICacheService cacheService)
        {
            _orderDetailsService = orderDetailsService;
            _configuration = configuration;
            _cacheService = cacheService;
        }


        //[HttpGet("GetOrderDetails")]
        //public async Task<IActionResult> Get(int id)
        //{
        //    var orderDetails = await _orderDetailsService.GetOrderDetails(id);

        //    if (orderDetails == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(orderDetails);
        //}

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

        //[HttpGet("GetAllOrderDetails")]
        //public async Task<IActionResult> GetAllOrderDetails()
        //{
        //    var orderDetails = await _orderDetailsService.GetAllOrderDetails();

        //    return Ok(orderDetails);
        //}

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



        //[HttpPost("PostOrderDetails")]
        //public async Task<IActionResult> Post(OrderDetailsCreateDto OrderDetailsToCreate)
        //{
        //    await _orderDetailsService.CreateOrderDetails(OrderDetailsToCreate);

        //    return Ok("OrderDetails created successfully!");
        //}

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
            await _orderDetailsService.UpdateOrderDetails(OrderDetailsToUpdate);

            return Ok("OrderDetails updated successfully!");
        }

        //[HttpPut("UpdateOrderDetails")]
        //public async Task<IActionResult> Update(OrderDetails OrderDetailsToUpdate)
        //{
        //    var key = "orderDetails_" + OrderDetailsToUpdate.Id;
        //    var cacheData = _cacheService.GetData<OrderDetails>(key);
        //    if (cacheData == null)
        //    {
        //        var orderDetails = await _orderDetailsService.GetOrderDetails(OrderDetailsToUpdate.Id);
        //        if (orderDetails == null)
        //        {
        //            return NotFound("OrderDetails not found!");
        //        }
        //        cacheData = orderDetails;
        //    }
        //    //update the fields
        //    cacheData.Id = OrderDetailsToUpdate.Id;
        //    cacheData.OrderData = OrderDetailsToUpdate.OrderData;
        //    cacheData.ProductId = OrderDetailsToUpdate.ProductId;
        //    cacheData.Count = OrderDetailsToUpdate.Count;
        //    cacheData.Price = OrderDetailsToUpdate.Price;
        //    //update in database
        //    await _orderDetailsService.UpdateOrderDetails(cacheData);
        //    //update in cache
        //    var expiryTime = DateTimeOffset.Now.AddMinutes(5);
        //    _cacheService.SetData<OrderDetails>(key, cacheData, expiryTime);
        //    return Ok("OrderDetails updated successfully!");
        //}




        [HttpDelete("DeleteOrderDetails")]
        public async Task<IActionResult> Delete(int id)
        {
            await _orderDetailsService.DeleteOrderDetails(id);

            return Ok("OrderDetails deleted successfully!");
        }
    }
}
