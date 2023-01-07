using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Models.DTOs.Review;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("GetReview")]
        public async Task<IActionResult> Get(int id)
        {
            var review = await _reviewService.GetReview(id);

            if (review == null)
            {
                return NotFound();
            }

            return Ok(review);
        }

       

        [HttpGet("GetReviews")]
        public async Task<IActionResult> GetReviews()
        {
            var reviews = await _reviewService.GetAllReviews();

            return Ok(reviews);
        }


        [HttpPost("PostReview")]
        public async Task<IActionResult> Post([FromForm] ReviewCreateDto ReviewToCreate)
        {
            await _reviewService.CreateReview(ReviewToCreate);

            return Ok("Review created successfully!");
        }

        [HttpPut("UpdateReview")]
        public async Task<IActionResult> Update(Review ReviewToUpdate)
        {
            await _reviewService.UpdateReview(ReviewToUpdate);

            return Ok("Review updated successfully!");
        }

        [HttpDelete("DeleteReview")]
        public async Task<IActionResult> Delete(int id)
        {
            await _reviewService.DeleteReview(id);

            return Ok("Review deleted successfully!");
        }
    }
}
