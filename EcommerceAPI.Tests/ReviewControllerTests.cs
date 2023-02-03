using EcommerceAPI.Controllers;
using EcommerceAPI.Models.DTOs.Review;
using EcommerceAPI.Services;
using EcommerceAPI.Services.IServices;
using EcommerceAPI.Validators.EntityValidators;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceAPI.Tests
{
    public class ReviewControllerTests
    {
        private readonly Mock<IReviewService> _reviewService;
        private readonly Mock<IValidator<ReviewCreateDto>> _createReviewValidator;
        private readonly Mock<IValidator<ReviewUpdateDto>> _updateReviewValidator;
        private readonly Mock<ILogger<ReviewController>> _logger;

        private ReviewController reviewController;
        public ReviewControllerTests()
        {
            _reviewService = new Mock<IReviewService>();
            _createReviewValidator = new Mock<IValidator<ReviewCreateDto>>();
            _updateReviewValidator = new Mock<IValidator<ReviewUpdateDto>>();
            _logger = new Mock<ILogger<ReviewController>>();
            reviewController = new ReviewController(_reviewService.Object, _createReviewValidator.Object, _updateReviewValidator.Object, _logger.Object);

        }
    }
}
