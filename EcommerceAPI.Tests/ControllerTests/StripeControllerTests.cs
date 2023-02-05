using Domain.Entities;
using EcommerceAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.DTOs.Stripe;
using Services.Services.IServices;
using System.Security.Claims;
using Xunit;

namespace EcommerceAPI.Tests.ControllerTests
{
    public class StripeControllerTests
    {
        private readonly Mock<IStripeAppService> _stripeService;
        private StripeController stripeController;

        public StripeControllerTests()
        {
            _stripeService = new Mock<IStripeAppService>();
            stripeController = new StripeController(_stripeService.Object);

        }


        [Fact]
        public async Task AddStripeCustomer_ReturnsOkResult_WithStripeCustomer_WhenUserIdIsNotNull()
        {
            // Arrange
            var mockStripeService = new Mock<IStripeAppService>();
            mockStripeService
                .Setup(x => x.AddStripeCustomerAsync(It.IsAny<string>(), It.IsAny<AddStripeCustomer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StripeCustomer("Test Name", "test@email.com", "test_customer_id"));

            var stripeController = new StripeController(mockStripeService.Object);

            var identity = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.NameIdentifier, "test_user_id")
             });
            var claimsPrincipal = new ClaimsPrincipal(identity);
            stripeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            var customer = new AddStripeCustomer();

            // Act
            var result = await stripeController.AddStripeCustomer(customer, default);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result.Result);
            var stripeCustomer = Assert.IsType<StripeCustomer>(okResult.Value);
        }

        [Fact]
        public async Task AddStripePayment_ReturnsOkResult_WithPaymentId_WhenUserIdIsNotNull()
        {
            // Arrange
            var mockStripeService = new Mock<IStripeAppService>();
            mockStripeService
                .Setup(x => x.AddStripePayment(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("test_payment_id");

            var stripeController = new StripeController(mockStripeService.Object);

            var identity = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.NameIdentifier, "test_user_id")
            });
            var claimsPrincipal = new ClaimsPrincipal(identity);
            stripeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            // Act
            var result = await stripeController.AddStripePayment("test_customer_id", "test_payment_id", "test_order_id");

            // Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);

        }


        [Fact]
        public async Task AttachPaymentMethodToCustomer_ReturnsOkResult_WhenPaymentMethodAttachedSuccessfully()
        {
            // Arrange
            _stripeService.Setup(x => x.AttachPaymentMethodToCustomer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await stripeController.AttachPaymentMethodToCustomer("customerId", "paymentMethodId");

            // Assert
            Assert.IsType(typeof(StatusCodeResult), result);
            _stripeService.Verify(x => x.AttachPaymentMethodToCustomer("customerId", "paymentMethodId"), Times.Once());
        }

        [Fact]
        public async Task AddPaymentMethod_ReturnsOkResult_WithPaymentMethod_WhenUserIdIsNotNull()
        {
            // Arrange
            var mockStripeService = new Mock<IStripeAppService>();
            mockStripeService
                .Setup(x => x.CreatePaymentMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new PaymentMethodEntity
                {
                    PaymentMethodId = "test_payment_method_id",
                    UserId = "test_user_id",
                    CardBrand = "Visa",
                    CardLastFour = "1234",
                    ExpMonth = 12,
                    ExpYear = 2030
                });

            var stripeController = new StripeController(mockStripeService.Object);

            var identity = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.NameIdentifier, "test_user_id")
            });
            var claimsPrincipal = new ClaimsPrincipal(identity);
            stripeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            // Act
            var result = await stripeController.AddPaymentMethod("test_card_number", "12", "2030", "123");

            // Assert
            var okResult = Assert.IsType<ActionResult<PaymentMethodEntity>>(result);
        }


        [Fact]
        public async Task GetPaymentMethodsByCustomer_ReturnsUnauthorizedResult_WhenUserIdIsNull()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var claimsPrincipal = new ClaimsPrincipal(identity);
            stripeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            // Act
            var result = stripeController.GetPaymentMethodsByCustomer();

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);

        }

        [Fact]
        public async Task GetPaymentMethodsByCustomer_ReturnsOkResultWithPaymentMethods_WhenUserIdIsValid()
        {
            // Arrange
            var identity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, "userId")
            });
            var claimsPrincipal = new ClaimsPrincipal(identity);
            stripeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            var paymentMethod = new PaymentMethodEntity
            {
                PaymentMethodId = "paymentMethodId",
                UserId = "userId",
                CardBrand = "Visa",
                CardLastFour = "1234",
                ExpMonth = 12,
                ExpYear = 2023
            };

            _stripeService.Setup(x => x.GetPaymentMethodsByCustomer("userId"))
                .Returns(new List<PaymentMethodEntity> { paymentMethod });

            // Act
            var result = stripeController.GetPaymentMethodsByCustomer();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            var paymentMethods = Assert.IsType<List<PaymentMethodEntity>>(objectResult.Value);
        }

        [Fact]
        public async Task DeletePaymentMethod_CallsDeletePaymentMethodMethod()
        {
            // Arrange
            var mockStripeService = new Mock<IStripeAppService>();
            mockStripeService.Setup(x => x.DeletePaymentMethod(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
            var stripeController = new StripeController(mockStripeService.Object);

            // Act
            await stripeController.DeletePaymentMethod("paymentMethodId");

            // Assert
            mockStripeService.Verify(x => x.DeletePaymentMethod("paymentMethodId"), Times.Once);

        }

        [Fact]
        public async Task UpdatePaymentMethodExpiration_CallsUpdatePaymentMethodExpirationMethod()
        {
            // Arrange
            var mockStripeService = new Mock<IStripeAppService>();
            mockStripeService.Setup(x => x.UpdatePaymentMethodExpiration(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);
            var stripeController = new StripeController(mockStripeService.Object);

            // Act
            await stripeController.UpdatePaymentMethodExpiration("paymentMethodId", 2022, 2);

            // Assert
            mockStripeService.Verify(x => x.UpdatePaymentMethodExpiration("paymentMethodId", 2022, 2), Times.Once);

        }
    }
}

