using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{

    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IConfiguration _configuration;

        public ProductController(IProductService productService, IConfiguration configuration)
        {
            _productService = productService;
            _configuration = configuration;
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
            await _productService.UpdateProduct(ProductToUpdate);

            return Ok("Product updated successfully!");
        }

        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProduct(id);

            return Ok("Product deleted successfully!");
        }
    }
}
