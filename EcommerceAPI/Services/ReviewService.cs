using AutoMapper;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Models.DTOs.Product;
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

        private async Task<Review> GetReview(int id)
        {
            Expression<Func<Review, bool>> expression = x => x.Id == id;
            var review = await _unitOfWork.Repository<Review>().GetById(expression).FirstOrDefaultAsync();

            return review;
        }
        public async Task<List<Review>> GetProductReviews(int productId)
        {
            Expression<Func<Review, bool>> expression = x => x.ProductId == productId;
            var reviews = _unitOfWork.Repository<Review>().GetByCondition(expression).ToList();

            return reviews;
        }

        public async Task<List<Review>> GetAllReviews()
        {
            var reviews = _unitOfWork.Repository<Review>().GetAll();
            return reviews.ToList();
        }


        public async Task<Review> CreateReview(ReviewCreateDto reviewToCreate)
        {
            
            var review = _mapper.Map<Review>(reviewToCreate);

            _unitOfWork.Repository<Review>().Create(review);
            _unitOfWork.Complete();
            return review;
        }
        public async Task UpdateReview(ReviewUpdateDto reviewToUpdate, string userId)
        {
            var review = await GetReview(reviewToUpdate.Id);        
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
    }
}
