using EcommerceAPI.Controllers;
using EcommerceAPI.Services;
using EcommerceAPI.Services.IServices;
using EcommerceAPI.Validators.EntityValidators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Services.DTOs.ShoppingCard;
using Services.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EcommerceAPI.Tests.ControllerTests
{
    public class ShoppingCardControllerTests
    {
        private readonly Mock<IShoppingCardService> _cardService;
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<ILogger<ShoppingCardController>> _logger;
        private ShoppingCardController shoppingCardController;
        public ShoppingCardControllerTests()
        {
            _cardService = new Mock<IShoppingCardService>();
            _configuration = new Mock<IConfiguration>();
            _logger = new Mock<ILogger<ShoppingCardController>>();
            shoppingCardController = new ShoppingCardController(_cardService.Object, _configuration.Object, _logger.Object);
        }

        [Fact]
        public async Task AddProductToCard_ReturnsOkResult_WhenCalledWithValidData()
        {
            var userData = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "testUserId") });
            shoppingCardController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal(userData) }
            };

            var result = await shoppingCardController.AddProductToCard(10, 100);

            Assert.IsType(typeof(OkObjectResult), result);
        }

        [Fact]
        public async Task AddProductToCard_ReturnsBadRequestResult_WhenExceptionIsThrown()
        {
            var userData = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "testUserId") });
            shoppingCardController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal(userData) }
            };

            _cardService.Setup(x => x.AddProductToCard(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new Exception("test exception"));

            var result = await shoppingCardController.AddProductToCard(10, 100);

            Assert.IsType(typeof(BadRequestObjectResult), result);
        }

        [Fact]
        public async Task RemoveProductFromCard_ReturnsOkResult_WhenCalledWithValidData()
        {
            var userData = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "testUserId") });
            shoppingCardController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal(userData) }
            };

            _cardService.Setup(x => x.RemoveProductFromCard(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var result = await shoppingCardController.RemoveProductFromCard(10);

            Assert.IsType(typeof(OkObjectResult), result);
        }

        [Fact]
        public async Task RemoveProductFromCard_ReturnsBadRequestResult_WhenAnErrorOccurs()
        {
            var userData = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "testUserId") });
            shoppingCardController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal(userData) }
            };

            _cardService.Setup(x => x.RemoveProductFromCard(It.IsAny<int>(), It.IsAny<string>()))
                .Throws(new Exception("Test Exception"));

            var result = await shoppingCardController.RemoveProductFromCard(10);

            Assert.IsType(typeof(BadRequestObjectResult), result);
        }

        [Fact]
        public async Task RemoveAllProductsFromCard_ReturnsOk_WhenUserIsAuthenticatedAndProductsRemovedSuccessfully()
        {
            // Arrange
            var userId = "userId";
            var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var controller = new ShoppingCardController(_cardService.Object, null, null);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            _cardService
                .Setup(x => x.RemoveAllProductsFromCard(userId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await controller.RemoveAllProductsFromCard();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RemoveAllProductsFromCard_ReturnsBadRequest_WhenRemovingProductsFails()
        {
            // Arrange
            var userId = "userId";
            var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var controller = new ShoppingCardController(_cardService.Object, null, null);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            _cardService
                .Setup(x => x.RemoveAllProductsFromCard(userId))
                .Throws(new Exception());

            // Act
            Exception exception = null;
            try
            {
                var result = await controller.RemoveAllProductsFromCard();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<Exception>(exception);
        }

        [Fact]
        public async Task ShoppingCardContent_ReturnsOkResult_WhenCalled()
        {
            // Arrange
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "123"));
            shoppingCardController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(claimsIdentity)
                }
            };
            var shoppingCardDetails = new ShoppingCardDetails();
            _cardService.Setup(x => x.GetShoppingCardContentForUser(It.IsAny<string>()))
                .ReturnsAsync(shoppingCardDetails);

            // Act
            var result = await shoppingCardController.ShoppingCardContent();

            // Assert
            Assert.IsType(typeof(OkObjectResult), result);
            var okResult = (OkObjectResult)result;
        }

        [Fact]
        public async Task ShoppingCardContent_ReturnsNotFoundResult_WhenNoDataIsFound()
        {
            // Arrange
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "123"));
            shoppingCardController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(claimsIdentity)
                }
            };
            _cardService.Setup(x => x.GetShoppingCardContentForUser(It.IsAny<string>()))
                .ReturnsAsync((ShoppingCardDetails)null);

            // Act
            var result = await shoppingCardController.ShoppingCardContent();

            // Assert
            Assert.IsType(typeof(NotFoundResult), result);
        }

        [Fact]
        public async Task IncreaseProductQuantity_ReturnsOkResult_WhenProductQuantityIsSuccessfullyIncreased()
        {
            // Arrange
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "123"));
            shoppingCardController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(claimsIdentity)
                }
            };
            _cardService.Setup(x => x.IncreaseProductQuantityInShoppingCard(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int?>()))
                            .Returns(Task.CompletedTask);

            // Act
            var result = await shoppingCardController.IncreaseProductQuantity(10, 1);

            // Assert
            Assert.IsType(typeof(OkObjectResult), result);
        }

        [Fact]
        public async Task IncreaseProductQuantity_ReturnsBadRequestResult_WhenErrorOccursWhileIncreasingProductQuantity()
        {
            // Arrange
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "123"));
            shoppingCardController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(claimsIdentity)
                }
            };
            _cardService.Setup(x => x.IncreaseProductQuantityInShoppingCard(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int?>()))
                .ThrowsAsync(new Exception("An error occurred while increasing product quantity"));

            // Act
            var result = await shoppingCardController.IncreaseProductQuantity(10, 1);

            // Assert
            Assert.IsType(typeof(BadRequestObjectResult), result);
        }

        [Fact]
        public async Task DecreaseProductQuantity_ReturnsOkResult_WhenDataIsSuccessfullyUpdated()
        {
            // Arrange
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "123"));
            shoppingCardController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(claimsIdentity)
                }
            };
            _cardService.Setup(x => x.DecreaseProductQuantityInShoppingCard(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int?>()))
            .Returns(Task.CompletedTask);
            // Act
            var result = await shoppingCardController.DecreaseProductQuantity(10, 1);

            // Assert
            Assert.IsType(typeof(OkObjectResult), result);
        }

        [Fact]
        public async Task DecreaseProductQuantity_ReturnsBadRequestResult_WhenExceptionIsThrown()
        {
            // Arrange
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "123"));
            shoppingCardController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(claimsIdentity)
                }
            };
            _cardService.Setup(x => x.DecreaseProductQuantityInShoppingCard(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int?>()))
            .Throws(new Exception());

            // Act
            var result = await shoppingCardController.DecreaseProductQuantity(10, 1);

            // Assert
            Assert.IsType(typeof(BadRequestObjectResult), result);
        }

    }
}
