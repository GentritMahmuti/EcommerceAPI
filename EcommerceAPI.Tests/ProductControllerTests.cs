using Amazon.Runtime.Internal.Util;
using AutoMapper;
using EcommerceAPI.Controllers;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services;
using EcommerceAPI.Services.IServices;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;


namespace ECommerceAPI.Tests
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _productService;
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<ILogger<ProductController>> _logger;
        private readonly Mock<IValidator<Product>> _productValidator;
        private ProductController productController;

        public ProductControllerTests()
        {
            _productService = new Mock<IProductService>();
            _configuration = new Mock<IConfiguration>();
            _logger = new Mock<ILogger<ProductController>>();
            _productValidator = new Mock<IValidator<Product>>();
            productController = new ProductController(_productService.Object, _configuration.Object, _productValidator.Object, _logger.Object);
        }


        [Fact]
        public async Task CreateOrderForProduct_ReturnsOkResult_WithSuccessMessage_WhenOrderIsCreated()
        {
            // Arrange
            var productId = 1;
            var count = 2;
            var addressDetails = new AddressDetails()
            {
                PhoheNumber = "+38345454545",
                StreetAddress = "Komandant Kumanova",
                City = "Lipjan",
                Country = "Kosove",
                PostalCode = "14000",
                Name = "Fatlinda",
                Email = "fatlinda.reqica@gmail.com"
            };
            _productService.Setup(x => x.CreateOrderForProduct("userId", productId, count, addressDetails)).Returns(Task.CompletedTask);
            productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.NameIdentifier, "userId")
            }, "mock"))
                }
            };

            //Act
            var result = await productController.CreateOrderForProduct(productId, count, addressDetails);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Order created!", okResult.Value);
            _productService.Verify(x => x.CreateOrderForProduct("userId", productId, count, addressDetails), Times.Once);
        }

        [Fact]
        public async Task GetProducts_ReturnsOkResult_WithExpectedProducts()
        {
            // Arrange
            var product1 = new Product { Id = 1, Name = "Telefon" };
            var product2 = new Product { Id = 2, Name = "Llaptop" };
            var productFilter = new ProductFilter { CategoryId = 1 };
            var productSort = new ProductSort { SortBy = "Price" };
            var expectedProducts = new List<Product> { product1, product2 };
            _productService.Setup(s => s.GetFilterProducts(productFilter, productSort)).Returns(Task.FromResult(expectedProducts));

            var result = await productController.GetProducts(productFilter, productSort);

            // Assert
            // Assert
            var okResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
            Assert.Equal(expectedProducts, okResult.Value);

            _productService.Verify(s => s.GetFilterProducts(productFilter, productSort), Times.Once);
        }


        [Fact]
        public async Task Get_ReturnsNotFoundResult_WhenProductIsNotFound()
        {
            // Arrange
            _productService.Setup(x => x.GetProduct(It.IsAny<int>()))
                .ReturnsAsync((Product)null);

            // Act
            var result = await productController.Get(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        public async Task Get_ReturnsOkResult_WithExpectedProduct()
        {
            // Arrange
            var productId = 1;
            var expectedProduct = new Product { Id = productId, Name = "Test product" };
            _productService.Setup(x => x.GetProduct(productId)).ReturnsAsync(expectedProduct);
            var controller = new ProductController(null, _configuration.Object, null, null);

            // Act
            var result = await controller.Get(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedProduct, okResult.Value);
            _productService.Verify(x => x.GetProduct(productId), Times.Once);
        }
        [Fact]
        public async Task GetAllProducts_ReturnsOkResult_WithExpectedProducts()
        {
            // Arrange
            var expectedProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1" },
                new Product { Id = 2, Name = "Product 2" }
            };
            _productService.Setup(x => x.GetAllProducts()).ReturnsAsync(expectedProducts);
            // Act
            var result = await productController.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedProducts, okResult.Value);
            _productService.Verify(x => x.GetAllProducts(), Times.Once());
        }

        [Fact]
        public async Task GetProducts_ReturnsNotFoundResult_WhenNoProductFound()
        {
            // Arrange
            _productService.Setup(x => x.GetAllProducts()).ReturnsAsync(new List<Product>());

            // Act
            var result = await productController.GetProducts();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            _productService.Verify(x => x.GetAllProducts(), Times.Once());
        }

        [Fact]
        public async Task GetProduct_ReturnsOkResult_WhenProductFound()
        {
            // Arrange
            _productService.Setup(x => x.GetProduct(It.IsAny<int>())).ReturnsAsync(new Product { Id = 1, Name = "Test Product" });

            // Act
            var result = await productController.Get(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var product = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(1, product.Id);
            Assert.Equal("Test Product", product.Name);
            _productService.Verify(x => x.GetProduct(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task Update_WithValidInput_ReturnsOkResult()
        {
            // Arrange
            var productToUpdate = new Product()
            {
                Id = 1,
                Name = "Test Product",
                Description = "Test Description",
                Price = 10.99
            };
            _productValidator.Setup(p => p.ValidateAsync(productToUpdate, default))
                 .ReturnsAsync(new ValidationResult());

            // Act
            var result = await productController.Update(productToUpdate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }
    


        [Fact]
        public async Task Delete_WithValidInput_ReturnsOkResult()
        {
            // Arrange
            int id = 1;
            _productService.Setup(x => x.DeleteProduct(id)).Returns(Task.CompletedTask);

            // Act
            var result = await productController.Delete(id);

            // Assert
            Assert.IsType(typeof(OkObjectResult), result);
        }


        [Fact]
        public async Task Delete_WithInvalidInput_ReturnsBadRequestResult()
        {
            // Arrange
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.DeleteProduct(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            var controller = new ProductController(mockProductService.Object, null, null, null);

            // Act
            var result = await controller.Delete(0);

            // Assert
            Assert.IsType(typeof(BadRequestObjectResult), result);
        }


        [Fact]
        public async Task UploadImage_WithValidInput_ReturnsOkResult()
        {
            // Arrange
            var fakeFile = new Mock<IFormFile>();
            _productService.Setup(x => x.UploadImage(fakeFile.Object, 1)).ReturnsAsync("http://fakeurl.com/image.jpg");

            // Act
            var result = await productController.UploadImage(fakeFile.Object, 1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Picture was uploaded sucessfully at the url: http://fakeurl.com/image.jpg", okResult.Value);
        }


        [Fact]
        public async Task UploadImage_WithException_ReturnsBadRequestResult()
        {
            // Arrange
            var file = new Mock<IFormFile>();
            _productService.Setup(p => p.UploadImage(file.Object, 1))
            .Throws(new Exception("An error occurred while uploading the image."));

            // Act
            var result = await productController.UploadImage(file.Object, 1);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal("An error occurred while uploading the image.", badRequestResult.Value);
        
        }

        [Fact]
        public async Task TestSearchElastic()
        {
            // Arrange
            var input = new SearchInputDto();
            var expectedResponse = new List<Product>();
            int pageIndex = 1;
            int pageSize = 10;

            _productService.Setup(x => x.SearchElastic(input, pageIndex, pageSize))
             .ReturnsAsync(expectedResponse);

            // Act
            var actualResponse = await productController.SearchElastic(input, pageIndex, pageSize);

            // Assert
            Assert.IsType(typeof(OkObjectResult), actualResponse);
        }


        [Fact]
        public async Task TestProductDiscount()
        {
            // Arrange
            int productId = 1;
            int discountPercentage = 50;
            var expectedResponse = new List<Product>();
            _productService.Setup(x => x.ProductDiscount(productId, discountPercentage));

            // Act
            var actualResponse = await productController.ProductDiscount(productId, discountPercentage);

            // Assert
            Assert.IsType(typeof(OkObjectResult), actualResponse);
            
        }
    }
}
