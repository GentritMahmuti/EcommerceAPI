using EcommerceAPI.Controllers;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.DTOs.Promotion;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace EcommerceAPI.Tests
{
    public class PromotionControllerTests
    {
        private readonly Mock<IPromotionService> _promotionService;
        private readonly Mock <IConfiguration> _configuration;
        private readonly Mock<IValidator<PromotionDto>> _promotionValidator;

        private PromotionController promotionController;

        public PromotionControllerTests()
        {
            _promotionService = new Mock<IPromotionService>();
            _configuration = new Mock<IConfiguration>();
            _promotionValidator = new Mock<IValidator<PromotionDto>>();
            promotionController = new PromotionController(_promotionService.Object, _configuration.Object, _promotionValidator.Object);
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
            Assert.IsType<NotFoundResult>(result);
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

            var result = await promotionController.Post(promotionDto);
            Assert.IsType<OkObjectResult>(result);
        }

    }
}
