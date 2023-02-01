﻿using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using EcommerceAPI.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using EcommerceAPI.Models.DTOs.Order;

namespace EcommerceAPI.Controllers
{

    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        private readonly IValidator<Product> _productValidator;

        public ProductController(IProductService productService, IValidator<Product> productValidator, ILogger<ProductController> logger)
        {
            _productService = productService;
            _productValidator = productValidator;
            _logger = logger;
        }

        [Authorize(Roles = "LifeUser, LifeAdmin")]
        [HttpPost("CreateOrderForProduct")]
        public async Task<IActionResult> CreateOrderForProduct(int productId, int count, AddressDetails addressDetails)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            await _productService.CreateOrderForProduct(userId, productId, count, addressDetails);

            return Ok("Order created!");
        }


        [HttpGet("GetProduct")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _productService.GetProduct(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }


        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetAllProducts();

            return Ok(products);
        }

        [Authorize(Roles = "LifeUser, LifeAdmin")]
        [HttpGet("GetRecommendedProducts")]
        public async Task<IActionResult> GetRecommendedProducts(int pageIndex = 1, int pageSize = 10)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;
            var products = await _productService.GetRecommendedProducts(userId, pageIndex, pageSize);
            return Ok(products);
        }
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto productToCreate)
        {
            await _productService.CreateProduct(productToCreate);

            return Ok("Product created successfully!");
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> Update(ProductDto productToUpdate)
        {
            try
            {
                //await _productValidator.ValidateAndThrowAsync(productToUpdate);
                await _productService.UpdateProduct(productToUpdate);
                return Ok("Product updated successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error happened: '{ex.Message}'");
            }
        }

        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProduct(id);

            return Ok("Product deleted successfully!");
        }

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile file, int productId)
        {
            try
            {
                var url = await _productService.UploadImage(file, productId);
                return Ok($"Picture was uploaded sucessfully at the url: {url}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in uploading the image.");
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("SearchElastic")]
        public async Task<IActionResult> SearchElastic([FromQuery] SearchInputDto input, int pageIndex = 1, int pageSize = 10)
        {
            var response = await _productService.SearchElastic(input, pageIndex, pageSize);

            return Ok(response);
        }



        [HttpGet("GetAllElastic")]
        public async Task<IActionResult> GetAllElastic()
        {
            var products = await _productService.GetAllElastic();
            return Ok(products);
        }



        [HttpPut("UpdateElastic")]
        public async Task<IActionResult> UpdateElastic([FromBody] ProductDto product)
        {
            try
            {
                await _productService.UpdateElastic(product);
                return Ok("Products are updated successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error happened: '{ex.Message}'");
            }
        }

        [HttpDelete("DeleteAllElastic")]
        public async Task<IActionResult> DeleteAllElastic()
        {
            try
            {
                await _productService.DeleteAllElastic();
                return Ok("Products are deleted successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error happened: '{ex.Message}'");
            }
        }

        [HttpPut("ProductDiscount")]
        public async Task<IActionResult> ProductDiscount(int productId, [Range(1, 100, ErrorMessage = "Value for discount percentage must be between 1 and 100.")] int discountPercentage)
        {
            await _productService.ProductDiscount(productId, discountPercentage);
            return Ok("Product discounted successfully");
        }

        [HttpPut("RemoveProductDiscount")]
        public async Task<IActionResult> RemoveProductDiscount(int productId)
        {
            await _productService.RemoveProductDiscount(productId);
            return Ok("The product discount was removed successfully");
        }
    }
}
