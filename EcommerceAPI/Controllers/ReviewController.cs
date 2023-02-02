using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.DTOs.Review;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services;
using EcommerceAPI.Services.IServices;
using EcommerceAPI.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IValidator<ReviewCreateDto> _createReviewValidator;
        private readonly IValidator<ReviewUpdateDto> _updateReviewValidator;
        private readonly ILogger<ReviewController> _logger;
        public ReviewController(IReviewService reviewService, IValidator<ReviewCreateDto> createReviewValidator, IValidator<ReviewUpdateDto> updateReviewValidator, ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _createReviewValidator = createReviewValidator;
            _updateReviewValidator = updateReviewValidator;
            _logger = logger;
        }

        /// <summary>
        /// Gets reviews that a specific user has done.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize("Roles = LifeAdmin")]
        [HttpGet("GetUserReviews")]
        public async Task<IActionResult> GetUserReviews(string userId)
        {
            var reviews = await _reviewService.GetUserReviews(userId);
            return Ok(reviews);
        }

        /// <summary>
        /// Gets reviews that you(client) have done.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "LifeUser, LifeAdmin")]
        [HttpGet("GetYourReviews")]
        public async Task<IActionResult> GetYourReviews()
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;
            var reviews = await _reviewService.GetUserReviews(userId);
            return Ok(reviews);
        }


        /// <summary>
        /// Gets reviews about a product.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeAdmin, LifeUser")]
        [HttpGet("GetProductReviews")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var reviews = await _reviewService.GetProductReviews(productId);
            return Ok(reviews);
        }

      
        [Authorize(Roles = "LifeAdmin")]
        [HttpGet("GetAllReviews")]
        public async Task<IActionResult> GetReviews()
        {
            var reviews = await _reviewService.GetAllReviews();

            return Ok(reviews);
        }

        /// <summary>
        /// Creates a new review.
        /// </summary>
        /// <param name="ReviewToCreate"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeUser, LifeAdmin")]
        [HttpPost("PostReview")]
        public async Task<IActionResult> Post([FromForm] ReviewCreateDto ReviewToCreate)
        {
            await _createReviewValidator.ValidateAndThrowAsync(ReviewToCreate);
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _reviewService.CreateReview(userId, ReviewToCreate);

            return Ok("Review created successfully!");
        }
        

        /// <summary>
        /// Updates a specific review.
        /// </summary>
        /// <param name="ReviewToUpdate"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeUser, LifeAdmin")]
        [HttpPut("UpdateReview")]
        public async Task<IActionResult> Update(ReviewUpdateDto ReviewToUpdate)
        {
            try
            {
                await _updateReviewValidator.ValidateAndThrowAsync(ReviewToUpdate);
                var userData = (ClaimsIdentity)User.Identity;
                var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _reviewService.UpdateReview(ReviewToUpdate, userId);
                return Ok("Review updated successfully!");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{nameof(ReviewController)} - Error when updating review.");
                return BadRequest("An error happened: " + e.Message);
            }

        }


        /// <summary>
        /// Deletes a specific review.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeUser, LifeAdmin")]
        [HttpDelete("DeleteReview")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userData = (ClaimsIdentity)User.Identity;
                var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _reviewService.DeleteReview(id, userId);
                return Ok("Review deleted successfully!");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{nameof(ReviewController)} - Error when deleting review.");
                return BadRequest("An error happened: " + e.Message);
            }
        }

        /// <summary>
        /// Gives possibility to admins to delete a comment of a review, in case it is unappropiate, harmful, racist etc.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeAdmin")]
        [HttpDelete("DeleteReviewComment")]
        public async Task<IActionResult> DeleteReviewComment(int id)
        {
            try
            {
                await _reviewService.DeleteReviewComment(id);
                return Ok("Review comment deleted successfully!");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{nameof(ReviewController)} - Error when deleting review comment.");
                return BadRequest("An error happened: " + e.Message);
            }
        }

    }
}
