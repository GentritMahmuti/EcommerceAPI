using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Review;
using Services.Services.IServices;
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
        /// <param name="userId">The user Id for which the reviews are fetched</param>
        /// <returns>A list of all the reviews for the user</returns>
        /// <response code="200">A list of all the reviews for the user is returned successfully</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Review</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" role to access.
        /// </remarks>
        [Authorize(Roles = "LifeAdmin")]
        [HttpGet("GetUserReviews")]
        public async Task<IActionResult> GetUserReviews(string userId)
        {
            var reviews = await _reviewService.GetUserReviews(userId);
            return Ok(reviews);
        }




        /// <summary>
        /// Returns a list of all reviews posted by the current user.
        /// </summary>
        /// <returns>A list of reviews posted by the current user</returns>
        /// <response code="200">A list of reviews is returned successfully</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Review</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeUser" role to access.
        /// </remarks>
        [Authorize]
        [HttpGet("GetYourReviews")]
        public async Task<IActionResult> GetYourReviews()
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;
            var reviews = await _reviewService.GetUserReviews(userId);
            return Ok(reviews);
        }


        /// <summary>
        /// Returns reviews about a specific product.
        /// </summary>
        /// <param name="productId">The id of the product to retrieve reviews for</param>
        /// <returns>A list of reviews about the product</returns>
        /// <response code="200">A list of reviews about the product is returned successfully</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Review</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" or "LifeUser" role to access.
        /// </remarks>
        [Authorize]
        [HttpGet("GetProductReviews")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var reviews = await _reviewService.GetProductReviews(productId);
            return Ok(reviews);
        }


        /// <summary>
        /// Returns a list of all the reviews.
        /// </summary>
        /// <returns>A list of all the reviews</returns>
        /// <response code="200">A list of all the reviews is returned successfully</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Review</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" role to access.
        /// </remarks>
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
        /// <param name="ReviewToCreate">The data required to create a new review</param>
        /// <returns>A message indicating if the review was created successfully</returns>
        /// <response code="200">The review was created successfully</response>
        /// <response code="400">An error occurred while creating the review</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Review</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeUser" or "LifeAdmin" role to access.
        /// </remarks>
        [Authorize]
        [HttpPost("CreateReview")]
        public async Task<IActionResult> Create([FromForm] ReviewCreateDto ReviewToCreate)
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
        /// <param name="ReviewToUpdate">The review to be updated</param>
        /// <returns>A message indicating if the review was updated successfully</returns>
        /// <response code="200">The review was updated successfully</response>
        /// <response code="400">An error occurred while updating the review</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <response code="404">If the review could not be found</response>
        /// <response code="406">If the review data is invalid</response>
        /// <tags>Review</tags>
        /// <remarks>
        /// This action requires authentication and either the "LifeAdmin" or "LifeUser" role to access.
        /// </remarks>
        [Authorize]
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
        /// <param name="id">The id of the review to delete</param>
        /// <returns>A message indicating if the review was deleted successfully</returns>
        /// <response code="200">The review was deleted successfully</response>
        /// <response code="400">An error occurred while deleting the review</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Review</tags>
        /// <remarks>
        /// This action requires authentication and either the "LifeAdmin" role or the "LifeUser" role to access.
        /// </remarks>
        [Authorize]
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
        /// Deletes a review comment
        /// </summary>
        /// <param name="id">The id of the review comment to delete</param>
        /// <returns>A message indicating if the review comment was deleted successfully</returns>
        /// <response code="200">The review comment was deleted successfully</response>
        /// <response code="400">An error occurred while deleting the review comment</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Review Comment</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" role to access.
        /// </remarks>
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
