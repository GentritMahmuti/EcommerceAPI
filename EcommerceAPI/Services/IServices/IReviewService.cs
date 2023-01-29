using EcommerceAPI.Models.DTOs.Review;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface IReviewService
    {
        Task<List<ReviewDetailsDto>> GetUserReviews(string userId);
        Task<List<ReviewDetailsDto>> GetProductReviews(int productId);
        Task<List<ReviewDetailsDto>> GetAllReviews();
        Task <Review> CreateReview(string userId, ReviewCreateDto reviewToCreate);
        Task UpdateReview(ReviewUpdateDto reviewToUpdate, string userId);
        Task DeleteReview(int id, string userId);
        Task DeleteReviewComment(int id);

    }
}
