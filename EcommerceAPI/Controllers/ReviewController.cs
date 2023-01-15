using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.DTOs.Review;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services;
using EcommerceAPI.Services.IServices;
using EcommerceAPI.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IValidator<ReviewCreateDto> _reviewValidator;
        private readonly ICacheService _cacheService;
        public ReviewController(IReviewService reviewService, IValidator<ReviewCreateDto> reviewValidator, ICacheService cacheService)
        {
            _reviewService = reviewService;
            _reviewValidator = reviewValidator;
            _cacheService = cacheService;
        }

        [HttpGet("GetProductReviews")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var cacheData = _cacheService.GetData<List<Review>>($"reviews-{productId}");
            if (cacheData != null)
            {
                return Ok(cacheData);
            }
            else
            {
                var reviews = await _reviewService.GetProductReviews(productId);

                if (reviews == null)
                {
                    return NotFound();
                }

                var expiryTime = DateTimeOffset.Now.AddMinutes(5);
                _cacheService.SetData<List<Review>>($"reviews-{productId}", reviews, expiryTime);
                return Ok(reviews);
            }
        }


  //      [Authorize(Roles = "LifeAdmin")]
        [HttpGet("GetAllReviews")]
        public async Task<IActionResult> GetReviews()
        {
            var cacheData = _cacheService.GetData<List<Review>>("reviews");
            if (cacheData != null && cacheData.Count > 0)
            {
                return Ok(cacheData);
            }
            else
            {
                var reviews = await _reviewService.GetAllReviews();
                var expiryTime = DateTimeOffset.Now.AddMinutes(5);
                _cacheService.SetData<List<Review>>("reviews", reviews, expiryTime);
                return Ok(reviews);
            }
        }

  //      [Authorize(Roles = "LifeUser")]
        [HttpPost("PostReview")]
        public async Task<IActionResult> Post([FromForm] ReviewCreateDto ReviewToCreate)
        {
            await _reviewValidator.ValidateAndThrowAsync(ReviewToCreate);
            var review = await _reviewService.CreateReview(ReviewToCreate);
            var expiryTime = DateTimeOffset.Now.AddMinutes(5);
            var key = $"review-{review.Id}";
            _cacheService.SetData<Review>(key, review, expiryTime);
            return Ok("Review created successfully!");
        }



        [Authorize(Roles = "LifeUser")]
        [HttpPut("UpdateReview")]
        public async Task<IActionResult> Update(Review ReviewToUpdate)
        {
            try
            {
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
            var key = "review_" + id;
            var cacheData = _cacheService.GetData<Review>(key);
            if (cacheData != null)
            {
                _cacheService.RemoveData(key);
            }
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

    }
}
