using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Services.IServices;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{

    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        private readonly IValidator<ProductDto> _productDtoValidator;
        private readonly IValidator<ProductCreateDto> _productCreateDtoValidator;
        public ProductController(IProductService productService, IValidator<ProductDto> productDtoValidator, ILogger<ProductController> logger,IValidator<ProductCreateDto> productCreateDtoValidator)
        {
            _productService = productService;
            _productDtoValidator = productDtoValidator;
            _logger = logger;
            _productCreateDtoValidator = productCreateDtoValidator;
        }

        


        /// <summary>
        /// Gets a specific product by id!
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A product</returns>
        [Authorize(Roles = "LifeAdmin, LifeUser")]
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

        [Authorize(Roles = "LifeAdmin")]
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProducts();

            if (products == null || !products.Any())
                return NotFound();

            return Ok(products);
        }

        /// <summary>
        /// Gets products in paginated form!
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of products</returns>
        [Authorize(Roles = "LifeUser, LifeAdmin")]
        [HttpGet("GetProductsPaginated")]
        public async Task<IActionResult> GetProductsPaginated(int pageIndex = 1, int pageSize = 10)
        {
            var products = await _productService.GetProductsPaginated(pageIndex, pageSize);
            return Ok(products);
        }

        /// <summary>
        /// Gets products based on the categories of the products which user has reviewed or sorts by popularity!
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of products</returns>
        [Authorize(Roles = "LifeUser, LifeAdmin")]
        [HttpGet("GetRecommendedProducts")]
        public async Task<IActionResult> GetRecommendedProducts(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var userData = (ClaimsIdentity)User.Identity;
                var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;
                var products = await _productService.GetRecommendedProducts(userId, pageIndex, pageSize);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)} - Error while getting recommended products!");
                return BadRequest($"An error happened: '{ex.Message}'");
            }
        }

        /// <summary>
        /// Creates a product!
        /// </summary>
        /// <param name="productToCreate"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeAdmin")]
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto productToCreate)
        {
            try
            {
                await _productCreateDtoValidator.ValidateAndThrowAsync(productToCreate);
                await _productService.CreateProduct(productToCreate);

                return Ok("Product created successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)} - Error while creating product!");
                return BadRequest($"An error happened: '{ex.Message}'");
            }
        }

        /// <summary>
        /// Creates products from list!
        /// </summary>
        /// <param name="productsToCreate"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeAdmin")]
        [HttpPost("CreateProducts")]
        public async Task<IActionResult> CreateProducts([FromBody]List<ProductCreateDto> productsToCreate)
        {
            try
            {
                await _productService.CreateProducts(productsToCreate);
                return Ok("Products are created successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)} - Error while creating products from list!");
                return BadRequest($"An error happened: '{ex.Message}'");
            }
        }

        /// <summary>
        /// Creates products from a csv file!
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeAdmin")]
        [HttpPost("CreateProductsFromCsv")]
        public async Task<IActionResult> CreateProductsFromCsv(IFormFile file)
        {
            try
            {
                await _productService.CreateProductsFromCsv(file);
                return Ok("Products are created successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)} - Error while creating products from file!");
                return BadRequest($"An error happened: '{ex.Message}'");
            }
        }

        /// <summary>
        /// Updates a product by id!
        /// </summary>
        /// <param name="productToUpdate"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeAdmin")]
        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> Update(ProductDto productToUpdate)
        {
            try
            {
                await _productDtoValidator.ValidateAndThrowAsync(productToUpdate);
                await _productService.UpdateProduct(productToUpdate);
                return Ok("Product updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)} - Error while updating product!");
                return BadRequest($"An error happened: '{ex.Message}'");
            }
        }

        /// <summary>
        /// Deletes a product by id!
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeAdmin")]
        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid product id");
            }
            try
            {
                await _productService.DeleteProduct(id);
                return Ok("Product deleted successfully!");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)} - Error while deleting product!");
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Uploads an image to blob and sets as ImageUrl of the product with specific id!
        /// </summary>
        /// <param name="file"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeAdmin")]
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
                _logger.LogError(ex, $"{nameof(ProductController)} - Error in uploading the image.");
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Searches for a product by title, also filters products if given minPrice and maxPrice!
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>Paginated result of products!</returns>
        [Authorize(Roles = "LifeUser, LifeAdmin")]
        [HttpGet("SearchElastic")]
        public async Task<IActionResult> SearchElastic([FromQuery] SearchInputDto input, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var response = await _productService.SearchElastic(input, pageIndex, pageSize);
                return Ok(response);
            }catch(Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)} - Error while searching in elastic.");
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Gets all products from elastic!
        /// </summary>
        /// <returns>List of products</returns>
        [Authorize(Roles = "LifeAdmin")]
        [HttpGet("GetAllElastic")]
        public async Task<IActionResult> GetAllElastic()
        {
            try
            {
                var products = await _productService.GetAllElastic();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)} - Error in uploading the image.");
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "LifeAdmin")]
        [HttpPut("UpdateElastic")]
        public async Task<IActionResult> UpdateElastic([FromBody] ProductDto product)
        {
            try
            {
                await _productDtoValidator.ValidateAndThrowAsync(product);
                await _productService.UpdateElastic(product);
                return Ok("Products are updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)} - Error when deleting products from elastic!");
                return BadRequest($"An error happened: '{ex.Message}'");
            }
        }


        
        [Authorize(Roles = "LifeAdmin")]
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
                _logger.LogError(ex, $"{nameof(ProductController)} - Error when deleting products from elastic!");
                return BadRequest($"An error happened: '{ex.Message}'");
            }
        }


        /// <summary>
        /// Sets a discount to a product with specific id!
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="discountPercentage"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeAdmin")]
        [HttpPut("ProductDiscount")]
        public async Task<IActionResult> ProductDiscount(int productId, [Range(1, 100, ErrorMessage = "Value for discount percentage must be between 1 and 100.")] int discountPercentage)
        {
            try
            {
                await _productService.ProductDiscount(productId, discountPercentage);
                return Ok("Product discounted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)} - Error when applying product discount!");
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Removes the discount from a product!
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeAdmin")]
        [HttpPut("RemoveProductDiscount")]
        public async Task<IActionResult> RemoveProductDiscount(int productId)
        {
            try
            {
                await _productService.RemoveProductDiscount(productId);
                return Ok("The product discount was removed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductController)} - Error when removing product discount!");
                return BadRequest(ex.Message);
            }
        }
    }
}
