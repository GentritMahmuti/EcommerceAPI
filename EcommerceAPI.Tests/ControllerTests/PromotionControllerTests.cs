using EcommerceAPI.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Services.DTOs.Promotion;
using Services.Services.IServices;
using Xunit;

namespace EcommerceAPI.Tests.ControllerTests
{
    public class PromotionControllerTests
    {
        private readonly Mock<IPromotionService> _promotionService;
        private readonly Mock<IValidator<PromotionDto>> _promotionValidator;
        private readonly Mock<ILogger<PromotionController>> _logger;

        private PromotionController promotionController;

        public PromotionControllerTests()
        {
            _promotionService = new Mock<IPromotionService>();
            _promotionValidator = new Mock<IValidator<PromotionDto>>();
            _logger = new Mock<ILogger<PromotionController>>();
            promotionController = new PromotionController(_promotionService.Object,  _promotionValidator.Object, _logger.Object);
        }

        [Fact]
        public async Task Get_WhenCalled_ReturnsOkResult()
        {
            //Arrange
            int id = 1;
            _promotionService.Setup(m => m.GetPromotionDetails(id)).ReturnsAsync(new PromotionDetailsDto());

            //Act
            var result = await promotionController.Get(id);

            //Assert
            var okResult = result as OkObjectResult;
        }

        [Fact]
        public async Task Get_WhenCalledWithInvalidId_ReturnsNotFoundResult()
        {
            //Arrange
            int id = 0;
            _promotionService.Setup(m => m.GetPromotionDetails(id)).ReturnsAsync((PromotionDetailsDto)null);

            //Act
            var result = await promotionController.Get(id);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Test1_GetPromotions_ReturnsOkObjectResult_WhenCalled()
        {
            //Arrange
            _promotionService.Setup(x => x.GetAllPromotions()).ReturnsAsync(new List<PromotionDetailsDto>());

            //Act
            var result = await promotionController.GetPromotions();

            //Assert
            Assert.IsType<OkObjectResult>(result);
            _promotionService.Verify(x => x.GetAllPromotions(), Times.Once());
        }

        [Fact]
        public async Task Test2_GetPromotions_ReturnsBadRequestObjectResult_WhenExceptionOccurs()
        {
            _promotionService.Setup(x => x.GetAllPromotions()).Throws(new Exception());

            //Act
            var result = await promotionController.GetPromotions();

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _promotionService.Verify(x => x.GetAllPromotions(), Times.Once());
        }

        [Fact]
        public async Task Post_Promotion_Success()
        {
            PromotionDto promotionDto = new PromotionDto()
            {
                Name = "Gjirafa",
                DiscountAmount = 10,
                StartDate = DateTime.Now.AddDays(-2),
                EndDate = DateTime.Now.AddDays(2)
            };

            var result = await promotionController.CreatePromotion(promotionDto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ReturnsOkResult_WhenDeletePromotionIsSuccessful()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await promotionController.Delete(id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal("Promotion deleted successfully!", okResult.Value);
        }

        [Fact]
        public async Task ReturnsBadRequestResult_WhenDeletePromotionThrowsException()
        {
            // Arrange
            var id = 1;
            _promotionService.Setup(x => x.DeletePromotion(It.IsAny<int>())).Throws(new Exception("Error deleting promotion"));

            // Act
            var result = await promotionController.Delete(id);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal("Error deleting promotion: Error deleting promotion", badRequestResult.Value);
        }




    }
}
