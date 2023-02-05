using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence.Repository.IRepository;
using Persistence.Repository;
using Persistence.UnitOfWork.IUnitOfWork;
using Persistence;
using Services.Services;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Xunit;
using Services.DTOs.User;

namespace EcommerceAPI.Tests.IntegrationTests
{
    public class UserServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Mock<ILogger<UserService>> _loggerMock = new();
        private readonly IECommerceRepository<User> _repository;
        private readonly EcommerceDbContext dbContext;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            DbContextOptionsBuilder<EcommerceDbContext> dbOptions = new DbContextOptionsBuilder<EcommerceDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            dbContext = new EcommerceDbContext(dbOptions.Options);
            _unitOfWork = new UnitOfWork(dbContext);
            _repository = new ECommerceRepository<User>(dbContext);
            _userService = new UserService(_unitOfWork, _loggerMock.Object);
        }

        [Fact]
        public async void GetAllUsers_ShouldReturnAllUsers()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<EcommerceDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new EcommerceDbContext(options))
            {
                var user1 = new User
                {
                    Id = "1",
                    FirsName = "Elmedina",
                    LastName = "Lahu",
                    Email = "elmedina@email.com",
                    DateOfBirth = new DateTime(1980, 01, 01),
                    Gender = "Female",
                    PhoneNumber = "1234567890"
                };
                var user2 = new User
                {
                    Id = "2",
                    FirsName = "Fatlinda",
                    LastName = "Reqica",
                    Email = "fatlinda@email.com",
                    DateOfBirth = new DateTime(1985, 01, 01),
                    Gender = "Female",
                    PhoneNumber = "0987654321"
                };

                context.Users.Add(user1);
                context.Users.Add(user2);
                context.SaveChanges();

                var unitOfWork = new UnitOfWork(context);
                var userService = new UserService(unitOfWork, _loggerMock.Object);

                // Act
                var result = await userService.GetAllUsers();

                // Assert
                Assert.NotNull(result);
                Assert.IsType<List<User>>(result);
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async Task GetUser_WithNonExistentId_ThrowsException()
        {
            // Arrange
            var userService = new UserService(_unitOfWork, null);
            string nonExistentId = "non-existent-id";

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(() => userService.GetUser(nonExistentId));

            // Assert
            Assert.Equal("A user with this ID doesn't exist.", exception.Message);
        }

        [Fact]
        public async Task UpdateUser_InvalidUserId_ShouldThrowException()
        {
            // Arrange
            UserDto userToUpdate = new UserDto()
            {
                UserId = "invalid",
                FirsName = "Fatlinda",
                LastName = "Reqica",
                Email = "fatlinda@email.com",
                DateOfBirth = new DateTime(1985, 01, 01),
                Gender = "Female",
                PhoneNumber = "0987654321"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.UpdateUser(userToUpdate));
        }

        [Fact]
        public async Task UpdateUser_InvalidEmail_ShouldThrowException()
        {
            // Arrange
            UserDto userToUpdate = new UserDto()
            {
                UserId = "1",
                FirsName = "Fatlinda",
                LastName = "Reqica",
                Email = "fatlinda@email.com",
                DateOfBirth = new DateTime(1985, 01, 01),
                Gender = "Female",
                PhoneNumber = "0987654321"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.UpdateUser(userToUpdate));
        }

        [Fact]
        public async Task DeleteUser_NonExistingUser_ShouldThrowException()
        {
            // Arrange
            string userId = "99";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _userService.DeleteUser(userId));
            Assert.Equal("A user with this ID doesn't exist.", exception.Message);
        }



    }
}
