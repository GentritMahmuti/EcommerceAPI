using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using EcommerceAPI.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Controllers
{

    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductController> _logger;
        private readonly IValidator<Product> _productValidator;

        public ProductController(IProductService productService, IConfiguration configuration, IValidator<Product> productValidator, ILogger<ProductController> logger)
        {
            _productService = productService;
            _configuration = configuration;
            _productValidator = productValidator;
            _logger = logger;
        }


        // GET: api/products
        [HttpGet("GetFilterProducts")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
            [FromQuery] ProductFilter filter,
            [FromQuery] ProductSort sort)
        {
            return await _productService.GetFilterProducts(filter, sort);
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

        //[HttpGet("GetProductWithIncludes")]
        //public async Task<IActionResult> GetIncludes(int id)
        //{
        //    var product = await _productService.GetWithIncludes(id);

        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(product);
        //}

        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var categories = await _productService.GetAllProducts();

            return Ok(categories);
        }

        //[HttpGet("ProductsListView")]
        //public async Task<IActionResult> ProductsListView(string? search, int categoryId = 0, int page = 1, int pageSize = 10)
        //{
        //    var products = await _productService.ProductsListView(search, page, pageSize, categoryId);

        //    return Ok(products);
        //}

        [HttpPost("PostProduct")]
        public async Task<IActionResult> Post([FromForm] ProductCreateDto ProductToCreate)
        {
            await _productService.CreateProduct(ProductToCreate);

            return Ok("Product created successfully!");
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> Update(Product ProductToUpdate)
        {
            try
            {
                await _productValidator.ValidateAndThrowAsync(ProductToUpdate);
                await _productService.UpdateProduct(ProductToUpdate);
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
        public async Task<IActionResult> SearchElastic([FromQuery] SearchInputDto input, int pageIndex, int pageSize)
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
