using Domain.Entities;
using EcommerceAPI.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Services.DTOs.Order;
using Services.DTOs.ShoppingCard;
using Services.Services.IServices;
using System.Security.Claims;
using Xunit;

namespace EcommerceAPI.Tests.ControllerTests
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderService> _orderService;
        private readonly Mock<IValidator<AddressDetails>> _addressDetailsValidator;
        private readonly Mock<ILogger<OrderController>> _logger;
        private OrderController orderController;

        public OrderControllerTests()
        {
            _orderService = new Mock<IOrderService>();
            _addressDetailsValidator = new Mock<IValidator<AddressDetails>>();
            _logger = new Mock<ILogger<OrderController>>();
            _orderService.Setup(os => os.GetCustomerOrderHistory(It.IsAny<string>())).Returns(new List<OrderData>());
            orderController = new OrderController(_orderService.Object, _logger.Object);
        }

        [Fact]
        public async Task ChangeOrderStatus_ReturnsOkResult_WithSuccessMessage_WhenOrderStatusIsChanged()
        {
            // Arrange
            var orderId = "1";
            var status = "shipped";
            _orderService.Setup(x => x.ChangeOrderStatus(orderId, status)).Returns(Task.CompletedTask);

            // Act
            var result = await orderController.ChangeOrderStatus(orderId, status);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal("Order status changed!", okResult.Value);
        }

        [Fact]
        public async Task ChangeOrderStatus_ReturnsBadRequestResult_WithErrorMessage_WhenOrderIdIsInvalid()
        {
            // Arrange
            var orderId = "";
            var status = "shipped";

            // Act
            var result = await orderController.ChangeOrderStatus(orderId, status);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal("Invalid order id or status", badRequestResult.Value);
        }


        [Fact]
        public async Task ChangeOrderStatus_ReturnsBadRequestResult_WithErrorMessage_WhenStatusIsInvalid()
        {
            // Arrange
            var orderId = "1";
            var status = "";

            // Act
            var result = await orderController.ChangeOrderStatus(orderId, status);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal("Invalid order id or status", badRequestResult.Value);
        }

        [Fact]
        public async Task ReturnsOKResult_WithOrderHistory_WhenUserIsAuthenticated()
        {
            // Arrange
            var orderHistory = new List<OrderData> { new OrderData { OrderId = "1", OrderDate = DateTime.Now, OrderFinalPrice = 100 } };
            var userId = "1";
            var userIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
            var claimsPrincipal = new ClaimsPrincipal(userIdentity);
            orderController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = claimsPrincipal } };
            _orderService.Object.GetCustomerOrderHistory(userId);
            _orderService
            .Setup(os => os.GetCustomerOrderHistory(It.IsAny<string>()))
            .Returns(orderHistory);

            // Act
            var result = await orderController.GetCustomerOrderHistory();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(orderHistory, okResult.Value);
        }

        [Fact]
        public async Task ReturnsBadRequestResult_WithErrorMessage_WhenAnExceptionOccurs()
        {
            // Arrange
            var errorMessage = "An error occurred while retrieving the customer's order history.";
            var userId = "1";
            var userIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) });
            var claimsPrincipal = new ClaimsPrincipal(userIdentity);
            orderController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = claimsPrincipal } };
            _orderService.Object.GetCustomerOrderHistory(userId);
            _orderService.Setup(os => os.GetCustomerOrderHistory(userId)).Throws(new Exception(errorMessage));


            // Act
            var result = await orderController.GetCustomerOrderHistory();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal(errorMessage, badRequestResult.Value);
        }

        [Fact]
        public async Task ReturnsOkResult_WhenUserIsAuthenticated()
        {
            // Arrange
            var userId = "12345";
            var email = "test@test.com";

            var mockOrderService = new Mock<IOrderService>();
            mockOrderService.Setup(x => x.CreateOrder(It.IsAny<string>(), It.IsAny<AddressDetails>(), It.IsAny<string>()))
            .Returns(Task.FromResult(true));

            var productSummaryModel = new ProductSummaryModel
            {
                AddressDetails = new AddressDetails
                {
                    Email = email
                },
                PromoCode = "promocode"
            };

            var claims = new List<Claim>
            {
                 new Claim(ClaimTypes.NameIdentifier, userId),
                 new Claim(ClaimTypes.Email, email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var orderController = new OrderController(mockOrderService.Object,  _logger.Object);
            orderController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = claimsPrincipal } };

            // Act
            var result = await orderController.CreateOrderFromShoppingCard(productSummaryModel);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ReturnsUnauthorizedResult_WhenUserIsNotAuthenticated_Test2()
        {
            // Arrange
            var mockOrderService = new Mock<IOrderService>();

            var productSummaryModel = new ProductSummaryModel
            {
                AddressDetails = new AddressDetails(),
                PromoCode = "promocode"
            };

            var orderController = new OrderController(mockOrderService.Object,  null);
            orderController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() } };

            // Act
            var result = await orderController.CreateOrderFromShoppingCard(productSummaryModel);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task ReturnsUnauthorizedResult_WhenUserIsNotAuthenticated_Test3()
        {
            // Arrange
            orderController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() } };

            // Act
            var result = await orderController.CreateOrderFromShoppingCard(new ProductSummaryModel());

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task ReturnsOkResult_WhenUserIsAuthenticatedAndInputIsValid_Test()
        {
            // Arrange
            var productId = 1;
            var count = 1;
            var addressDetails = new AddressDetails { Email = "test@email.com" };
            orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                { User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "userId") }, "mock")) }
            };
            // Act
            var result = await orderController.CreateOrderForProduct(productId, count, addressDetails, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Order created!", okResult.Value);
        }

    }


}

