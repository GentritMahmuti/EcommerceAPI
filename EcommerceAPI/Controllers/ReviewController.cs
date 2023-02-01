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
        public ReviewController(IReviewService reviewService, IValidator<ReviewCreateDto> createReviewValidator, IValidator<ReviewUpdateDto> updateReviewValidator)
        {
            _reviewService = reviewService;
            _createReviewValidator = createReviewValidator;
            _updateReviewValidator = updateReviewValidator;
        }

        [Authorize]
        [HttpGet("GetUserReviews")]
        public async Task<IActionResult> GetUserReviews(string userId)
        {
            var reviews = await _reviewService.GetUserReviews(userId);
            return Ok(reviews);
        }


        [Authorize(Roles = "LifeUser")]
        [HttpGet("GetYourReviews")]
        public async Task<IActionResult> GetYourReviews()
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;
            var reviews = await _reviewService.GetUserReviews(userId);
            return Ok(reviews);
        }

        [Authorize]
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

        [Authorize(Roles = "LifeUser")]
        [HttpPost("PostReview")]
        public async Task<IActionResult> Post([FromForm] ReviewCreateDto ReviewToCreate)
        {
            await _createReviewValidator.ValidateAndThrowAsync(ReviewToCreate);
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;
            await _reviewService.CreateReview(userId, ReviewToCreate);

            return Ok("Review created successfully!");
        }
        
        [Authorize(Roles = "LifeUser")]
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
                return BadRequest(e);
            }

        }

        [Authorize(Roles = "LifeUser")]
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
                return BadRequest(e);
            }
        }
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
                return BadRequest(e);
            }
        }

    }
}
