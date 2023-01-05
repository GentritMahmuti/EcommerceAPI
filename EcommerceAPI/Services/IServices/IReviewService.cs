using EcommerceAPI.Models.DTOs.Review;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface IReviewService
    {
        Task<Review> GetReview(int id);
        Task<List<Review>> GetAllReviews();
        Task CreateReview(ReviewCreateDto reviewToCreate);
        Task UpdateReview(Review reviewToUpdate);
        Task DeleteReview(int id);


    }
}
