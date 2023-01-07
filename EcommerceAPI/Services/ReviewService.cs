using AutoMapper;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.DTOs.Review;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Review> GetReview(int id)
        {
            Expression<Func<Review, bool>> expression = x => x.Id == id;
            var review = await _unitOfWork.Repository<Review>().GetById(expression).FirstOrDefaultAsync();

            return review;
        }

        public async Task<List<Review>> GetAllReviews()
        {
            var reviews = _unitOfWork.Repository<Review>().GetAll();
            return reviews.ToList();
        }


        public async Task CreateReview(ReviewCreateDto reviewToCreate)
        {
            var review = _mapper.Map<Review>(reviewToCreate);

            _unitOfWork.Repository<Review>().Create(review);
            _unitOfWork.Complete();

        }
        public async Task UpdateReview(Review reviewToUpdate)
        {
            var review = await GetReview(reviewToUpdate.Id);
            if (review == null)
            {
                throw new NullReferenceException("The review you're trying to update doesn't exist!");
            }
            review.Rating = reviewToUpdate.Rating;
            review.ReviewComment = reviewToUpdate.ReviewComment;

            _unitOfWork.Repository<Review>().Update(review);

            _unitOfWork.Complete();
        }

        public async Task DeleteReview(int id)
        {
            var review = await GetReview(id);
            if (review == null)
            {
                throw new NullReferenceException("The review you're trying to delete doesn't exist.");
            }

            _unitOfWork.Repository<Review>().Delete(review);
            _unitOfWork.Complete();

        }

       
    }
}
