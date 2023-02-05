using Domain.Entities;
using EcommerceAPI.Controllers;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Services.DTOs.Product;
using Services.Services.IServices;
using Xunit;


namespace EcommerceAPI.Tests.ControllerTests
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _productService;
        private readonly Mock<ILogger<ProductController>> _logger;
        private readonly Mock<IValidator<ProductDto>> _productValidator;
        private readonly Mock<IValidator<ProductCreateDto>> _productCreateDtoValidator;

        private ProductController productController;

        public ProductControllerTests()
        {
            _productService = new Mock<IProductService>();
            _logger = new Mock<ILogger<ProductController>>();
            _productValidator = new Mock<IValidator<ProductDto>>();
            _productCreateDtoValidator = new Mock<IValidator<ProductCreateDto>>();
            productController = new ProductController(_productService.Object, _productValidator.Object, _logger.Object, _productCreateDtoValidator.Object);
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
            var result = await productController.GetAllProducts();

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
            var result = await productController.GetAllProducts();

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
            var productToUpdate = new ProductDto()
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
