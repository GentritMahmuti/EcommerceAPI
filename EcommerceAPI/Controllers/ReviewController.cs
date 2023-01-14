using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.DTOs.Review;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services;
using EcommerceAPI.Services.IServices;
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
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("GetProductReviews")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var reviews = await _reviewService.GetProductReviews(productId);

            if (reviews == null)
            {
                return NotFound();
            }

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
            await _reviewService.CreateReview(ReviewToCreate);

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
