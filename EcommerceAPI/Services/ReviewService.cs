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

        
        public async Task<List<ReviewDetailsDto>> GetUserReviews(string userId)
        {
            var reviews = _unitOfWork.Repository<Review>().GetByCondition(x=>x.UserId.Equals(userId)).ToList();
            var reviewsDetails = _mapper.Map<List<Review>, List<ReviewDetailsDto>>(reviews);
            return reviewsDetails;
        }

        public async Task<List<ReviewDetailsDto>> GetProductReviews(int productId)
        {
            Expression<Func<Review, bool>> expression = x => x.ProductId == productId;
            var reviews = _unitOfWork.Repository<Review>().GetByCondition(expression).ToList();
            var reviewsDetails = _mapper.Map<List<Review>, List<ReviewDetailsDto>>(reviews);

            return reviewsDetails;
        }

        public async Task<List<ReviewDetailsDto>> GetAllReviews()
        {
            var reviews = _unitOfWork.Repository<Review>().GetAll().ToList();
            var reviewsDetails = _mapper.Map<List<Review>, List<ReviewDetailsDto>>(reviews);
            return reviewsDetails;
        }


        public async Task<Review> CreateReview(string userId, ReviewCreateDto reviewToCreate)
        {
            
            var review = _mapper.Map<Review>(reviewToCreate);
            review.UserId = userId;

            _unitOfWork.Repository<Review>().Create(review);
            _unitOfWork.Complete();
            return review;
        }
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

                _unitOfWork.Complete();
            }
            else
            {
                throw new Exception("You cannot update this review.");
            }
        }

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
                _unitOfWork.Complete();
            }
            else
            {
                throw new Exception("You cannot delete this review.");
            }
            
        }
        public async Task DeleteReviewComment(int id)
        {
            var review = await GetReview(id);
            if (review == null)
            {
                throw new NullReferenceException("The review you're trying to delete doesn't exist.");
            }
            review.ReviewComment = null;
            _unitOfWork.Repository<Review>().Update(review);

            _unitOfWork.Complete();

        }

        private async Task<Review> GetReview(int id)
        {
            Expression<Func<Review, bool>> expression = x => x.Id == id;
            var review = await _unitOfWork.Repository<Review>().GetById(expression).FirstOrDefaultAsync();
            return review;
        }
    }
}
