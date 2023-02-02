using AutoMapper;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.DTOs.Promotion;
using EcommerceAPI.Models.DTOs.Review;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Nest;
using System.Linq.Expressions;

namespace EcommerceAPI.Services
{
    public class ReviewService : IReviewService
    {
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        
        /// <summary>
        /// Gets reviews that a user has done.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List of review details.</returns>
        public async Task<List<ReviewDetailsDto>> GetUserReviews(string userId)
        {
            var reviews = await _unitOfWork.Repository<Review>().GetByCondition(x=>x.UserId.Equals(userId)).ToListAsync();
            var reviewsDetails = _mapper.Map<List<Review>, List<ReviewDetailsDto>>(reviews);
            return reviewsDetails;
        }


        /// <summary>
        /// Gets all reviews of product
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>List of review details.</returns>
        public async Task<List<ReviewDetailsDto>> GetProductReviews(int productId)
        {
            var reviews = await _unitOfWork.Repository<Review>().GetByCondition(x => x.ProductId == productId).ToListAsync();
            var reviewsDetails = _mapper.Map<List<Review>, List<ReviewDetailsDto>>(reviews);

            return reviewsDetails;
        }


        /// <summary>
        /// Gets all reviews.
        /// </summary>
        /// <returns>List of review details.</returns>
        public async Task<List<ReviewDetailsDto>> GetAllReviews()
        {
            var reviews = await _unitOfWork.Repository<Review>().GetAll().ToListAsync();
            var reviewsDetails = _mapper.Map<List<Review>, List<ReviewDetailsDto>>(reviews);
            return reviewsDetails;
        }

        /// <summary>
        /// Creates a new review.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="reviewToCreate"></param>
        /// <returns>The created review.</returns>
        public async Task<Review> CreateReview(string userId, ReviewCreateDto reviewToCreate)
        {
            
            var review = _mapper.Map<Review>(reviewToCreate);
            review.UserId = userId;

            _unitOfWork.Repository<Review>().Create(review);
            await _unitOfWork.CompleteAsync();
            return review;
        }

        /// <summary>
        /// Updates a specific review if it exists, else throws exception.
        /// </summary>
        /// <param name="reviewToUpdate"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task UpdateReview(ReviewUpdateDto reviewToUpdate, string userId)
        {
            var review = await GetReview(reviewToUpdate.ReviewId);        
            if (review == null)
            {
                throw new NullReferenceException("The review you're trying to update doesn't exist!");
            }
            if (userId.Equals(review.UserId))
            {
                review.Rating = reviewToUpdate.Rating;
                review.ReviewComment = reviewToUpdate.ReviewComment;

                _unitOfWork.Repository<Review>().Update(review);

                await _unitOfWork.CompleteAsync();
            }
            else
            {
                throw new Exception("You cannot update this review.");
            }
        }

        /// <summary>
        /// Deletes a specific review if it exists, else throws exception.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task DeleteReview(int id, string userId)
        {
            var review = await GetReview(id);
            if (review == null)
            {
                throw new NullReferenceException("The review you're trying to delete doesn't exist.");
            }
            if (userId.Equals(review.UserId))
            {
                _unitOfWork.Repository<Review>().Delete(review);
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                throw new Exception("You cannot delete this review.");
            }
            
        }

        /// <summary>
        /// Sets the comment of a specific review to null. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public async Task DeleteReviewComment(int id)
        {
            var review = await GetReview(id);
            if (review == null)
            {
                throw new NullReferenceException("The review you're trying to delete doesn't exist.");
            }
            review.ReviewComment = null;
            _unitOfWork.Repository<Review>().Update(review);

            await _unitOfWork.CompleteAsync();

        }

        private async Task<Review> GetReview(int id)
        {
            var review = await _unitOfWork.Repository<Review>().GetById(x => x.Id == id).FirstOrDefaultAsync();
            return review;
        }
    }
}
