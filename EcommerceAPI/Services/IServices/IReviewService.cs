using EcommerceAPI.Models.DTOs.Review;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface IReviewService
    {
        Task<List<Review>> GetProductReviews(int productId);
        Task<List<Review>> GetAllReviews();
        Task <Review> CreateReview(ReviewCreateDto reviewToCreate);
        Task UpdateReview(ReviewUpdateDto reviewToUpdate, string userId);
        Task DeleteReview(int id, string userId);


    }
}
