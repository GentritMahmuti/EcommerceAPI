using AutoMapper;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence;
using Persistence.Repository;
using Persistence.Repository.IRepository;
using Persistence.UnitOfWork.IUnitOfWork;
using Services.DTOs.Review;
using Services.Services;
using Xunit;

namespace EcommerceAPI.Tests.IntegrationTests
{

    public class ReviewServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly IECommerceRepository<Review> _repository;
        private readonly EcommerceDbContext dbContext;
        private readonly ReviewService _reviewService;

        public ReviewServiceTests()
        {
            DbContextOptionsBuilder<EcommerceDbContext> dbOptions = new DbContextOptionsBuilder<EcommerceDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            dbContext = new EcommerceDbContext(dbOptions.Options);
            _unitOfWork = new UnitOfWork(dbContext);
            _repository = new ECommerceRepository<Review>(dbContext);
            _reviewService = new ReviewService(_unitOfWork, _mapperMock.Object);
        }
        [Fact]
        public void GetUserReviews_WhenUserExist_ReturnReviewsDetails()
        {
            //Arrange
            var userId = Guid.NewGuid().ToString();
            var reviews = new List<Review>()
            {
                new Review()
                {
                    UserId = userId,
                    ProductId = 1,
                    Rating = 4,
                    ReviewComment = "test"
                }
            };
            _repository.Create(reviews[0]);
            dbContext.SaveChanges();
            _mapperMock.Setup(x => x.Map<List<Review>, List<ReviewDetailsDto>>(It.IsAny<List<Review>>())).Returns(new List<ReviewDetailsDto>());
            //Act
            var result = _reviewService.GetUserReviews(userId);

            //Arrange
            result.Should().NotBeNull().And.BeOfType<Task<List<ReviewDetailsDto>>>();
        }

        [Fact]
        public async Task GetUserReviews_WhenUserDoesNotExist_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            _mapperMock.Setup(x => x.Map<List<Review>, List<ReviewDetailsDto>>(It.IsAny<List<Review>>())).Returns(new List<ReviewDetailsDto>());

            // Act
            var result = await _reviewService.GetUserReviews(userId);

            // Assert
            result.Should().NotBeNull().And.BeOfType<List<ReviewDetailsDto>>().And.BeEmpty();

        }

        [Fact]
        public void GetProductReviews_WhenProductExist_ReturnReviewsDetails()
        {
            //Arrange
            int productId = 1;
            var reviews = new List<Review>()
            {
            new Review()
                {
            UserId = Guid.NewGuid().ToString(),
            ProductId = productId,
            Rating = 4,
            ReviewComment = "test"
                 }
            };
            _repository.Create(reviews[0]);
            dbContext.SaveChanges();
            _mapperMock.Setup(x => x.Map<List<Review>, List<ReviewDetailsDto>>(It.IsAny<List<Review>>())).Returns(new List<ReviewDetailsDto>());
            //Act
            var result = _reviewService.GetProductReviews(productId);

            //Arrange
            result.Should().NotBeNull().And.BeOfType<Task<List<ReviewDetailsDto>>>();
        }

        [Fact]
        public void GetProductReviews_WhenProductDoesNotExist_ReturnEmptyList()
        {
            //Arrange
            int productId = 1;
            var reviews = new List<Review>()
            {
                new Review()
                {
                      UserId = Guid.NewGuid().ToString(),
                      ProductId = 2,
                      Rating = 4,
                       ReviewComment = "test"
                }
            };
            _repository.Create(reviews[0]);
            dbContext.SaveChanges();
            _mapperMock.Setup(x => x.Map<List<Review>, List<ReviewDetailsDto>>(It.IsAny<List<Review>>())).Returns(new List<ReviewDetailsDto>());
            //Act
            var result = _reviewService.GetProductReviews(productId);

            //Arrange
            result.Result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllReviews_ReturnsAllReviews()
        {
            // Arrange
            var reviews = new List<Review>
            {
                new Review { ProductId = 1, UserId = "user1", Rating = 4, ReviewComment = "comment1" },
                new Review { ProductId = 2, UserId = "user2", Rating = 5, ReviewComment = "comment2" },
                new Review { ProductId = 3, UserId = "user3", Rating = 3, ReviewComment = "comment3" },
            };
            _repository.CreateRange(reviews);
            dbContext.SaveChanges();

            _mapperMock.Setup(x => x.Map<List<Review>, List<ReviewDetailsDto>>(It.IsAny<List<Review>>()))
                .Returns(new List<ReviewDetailsDto>());

            // Act
            var result = await _reviewService.GetAllReviews();

            // Assert
            result.Should().NotBeNull().And.BeOfType<List<ReviewDetailsDto>>();
        }

        [Fact]
        public async Task GetAllReviews_NoReviewsExist_ReturnsEmptyList()
        {
            // Arrange
            _mapperMock.Setup(x => x.Map<List<Review>, List<ReviewDetailsDto>>(It.IsAny<List<Review>>()))
                .Returns(new List<ReviewDetailsDto>());

            // Act
            var result = await _reviewService.GetAllReviews();

            // Assert
            result.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public async Task CreateReview_WhenReviewIsValid_ReturnsCreatedReview()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var reviewToCreate = new ReviewCreateDto()
            {
                ProductId = 1,
                Rating = 4,
                ReviewComment = "Test Comment"
            };

            _mapperMock.Setup(x => x.Map<Review>(It.IsAny<ReviewCreateDto>())).Returns(new Review());

            // Act
            var result = await _reviewService.CreateReview(userId, reviewToCreate);

            // Assert
            result.Should().NotBeNull().And.BeOfType<Review>();
            _repository.GetAll().Count().Should().Be(1);
        }

        [Fact]
        public async Task UpdateReview_ReviewDoesNotExist_ThrowsException()
        {
            // Arrange
            var reviewToUpdate = new ReviewUpdateDto()
            {
                ReviewId = 999,
                Rating = 5,
                ReviewComment = "updated"
            };

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _reviewService.UpdateReview(reviewToUpdate, "user1"));
        }

        [Fact]
        public async Task DeleteReview_ShouldDeleteReview()
        {
            // Arrange
            var review = new Review
            {
                UserId = "user1",
                ProductId = 1,
                Rating = 5,
                ReviewComment = "Jam e kenaqur me sherbimin"
            };
            _unitOfWork.Repository<Review>().Create(review);
            await _unitOfWork.CompleteAsync();

            // Act
            await _reviewService.DeleteReview(review.Id, "user1");

            // Assert
            var deletedReview = await _unitOfWork.Repository<Review>().GetById(r => r.Id == review.Id).FirstOrDefaultAsync();
            Assert.Null(deletedReview);
        }


        [Fact]
        public async Task DeleteReviewComment_ShouldThrowExceptionWhenReviewNotFound()
        {
            // Arrange
            var id = 100;

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _reviewService.DeleteReviewComment(id));
        }

        [Fact]
        public async Task DeleteReviewComment_ShouldThrowExceptionWhenNotFound()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _reviewService.DeleteReviewComment(100));
        }

        [Fact]
        public async Task GetReview_ShouldReturnCorrectReview()
        {
            // Arrange
            var review = new Review
            {
                UserId = "user1",
                ProductId = 1,
                Rating = 5,
                ReviewComment = "Test Description"
            };
            _unitOfWork.Repository<Review>().Create(review);
            await _unitOfWork.CompleteAsync();

            // Act
            var result = await _reviewService.GetReview(review.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(review.Id, result.Id);
            Assert.Equal(review.ProductId, result.ProductId);
            Assert.Equal(review.Rating, result.Rating);
            Assert.Equal(review.ReviewComment, result.ReviewComment);
        }

        [Fact]
        public async Task GetReview_ShouldThrowNullReferenceExceptionWhenReviewIsNotFound()
        {
            // Arrange
            int nonExistingId = 100;

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _reviewService.GetReview(nonExistingId));
        }

        [Fact]
        public async Task GetReview_ShouldThrowExceptionWithCorrectMessageWhenReviewIsNotFound()
        {
            // Arrange
            int nonExistingId = 100;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => _reviewService.GetReview(nonExistingId));
            Assert.Equal("The review you're trying to retrieve doesn't exist.", exception.Message);
        }










    }
}