using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Persistence;
using Persistence.Repository;
using Persistence.Repository.IRepository;
using Persistence.UnitOfWork.IUnitOfWork;
using Services.Services;
using Xunit;

namespace EcommerceAPI.Tests.IntegrationTests
{
    public class SavedItemServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Mock<ILogger<SavedItem>> _loggerMock = new();
        private readonly IECommerceRepository<SavedItem> _repository;
        private readonly EcommerceDbContext dbContext;
        private readonly SavedItemService _savedItemService;

        public SavedItemServiceTests()
        {
            DbContextOptionsBuilder<EcommerceDbContext> dbOptions = new DbContextOptionsBuilder<EcommerceDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            dbContext = new EcommerceDbContext(dbOptions.Options);
            _unitOfWork = new UnitOfWork(dbContext);
            _repository = new ECommerceRepository<SavedItem>(dbContext);
            _savedItemService = new SavedItemService(_unitOfWork, _loggerMock.Object);
        }

        [Fact]
        public async Task GetSavedItemsContent_WhenUserIdIsNullOrEmpty_ThrowsException()
        {
            // Arrange
            string userId = null;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _savedItemService.GetSavedItemsContent(userId));
            Assert.Equal("UserId cannot be null or empty.", exception.Message);
        }

        [Fact]
        public async Task GetSavedItemsContent_WhenUserIdDoesNotExist_ReturnsEmptyList()
        {
            // Arrange
            string userId = "user123";

            // Act
            var result = await _savedItemService.GetSavedItemsContent(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetSavedItemsContent_WhenUserIdExists_ReturnsListOfProducts()
        {
            // Arrange
            string userId = "user123";
            var product1 = new Product { Id = 1, Name = "Product1", Price = 10, Description = "", ImageUrl = "" };
            var product2 = new Product { Id = 2, Name = "Product2", Price = 20, Description = "", ImageUrl = "" };
            var savedItem1 = new SavedItem { SavedItemId = "1", UserId = userId, ProductId = product1.Id };
            var savedItem2 = new SavedItem { SavedItemId = "2", UserId = userId, ProductId = product2.Id };


            dbContext.Products.Add(product1);
            dbContext.Products.Add(product2);
            dbContext.SavedItems.Add(savedItem1);
            dbContext.SavedItems.Add(savedItem2);
            dbContext.SaveChanges();

            // Act
            var result = await _savedItemService.GetSavedItemsContent(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task AddProductToSavedItems_WhenCalledWithValidUserIdAndProductId_AddsProductToSavedItems()
        {
            // Arrange
            string userId = "user123";
            int productId = 1;
            var product = new Product { Id = productId, Name = "Product1", Price = 10, Description = "Product1 description", ImageUrl = "product1.jpg" };
            dbContext.Products.Add(product);
            dbContext.SaveChanges();

            // Act
            await _savedItemService.AddProductToSavedItems(userId, productId);
            var savedItems = await dbContext.SavedItems.Where(x => x.UserId == userId).ToListAsync();

            // Assert
            Assert.NotEmpty(savedItems);
            Assert.Equal(1, savedItems.Count);
            Assert.Equal(userId, savedItems[0].UserId);
            Assert.Equal(productId, savedItems[0].ProductId);
        }

        [Fact]
        public async Task AddProductToSavedItems_WhenCalledWithInvalidProductId_ThrowsException()
        {
            // Arrange
            string userId = "user123";
            int productId = 1;
            var exceptionMessage = "Product not found";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _savedItemService.AddProductToSavedItems(userId, productId));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public async Task AddProductToSavedItems_WhenCalledWithExistingProductId_DoesNotAddDuplicateProduct()
        {
            // Arrange
            string userId = "user123";
            int productId = 1;
            var product = new Product { Id = productId, Name = "Product1", Price = 10, Description = "Product1 description", ImageUrl = "product1.jpg" };
            var savedItem = new SavedItem { SavedItemId = "1", UserId = userId, ProductId = productId };

            dbContext.Products.Add(product);
            dbContext.SavedItems.Add(savedItem);
            dbContext.SaveChanges();

            // Act
            await _savedItemService.AddProductToSavedItems(userId, productId);
            var savedItems = await dbContext.SavedItems.Where(x => x.UserId == userId).ToListAsync();

            // Assert
            Assert.NotEmpty(savedItems);
            Assert.Equal(1, savedItems.Count);
            Assert.Equal(userId, savedItems[0].UserId);
            Assert.Equal(productId, savedItems[0].ProductId);
        }

        [Fact]
        public async Task GetProductFromSavedItems_WhenCalledWithValidProductId_ReturnsProductFromSavedItems()
        {
            // Arrange
            int productId = 1;
            string userId = "user123";
            var product = new Product { Id = productId, Name = "Product1", Price = 10, Description = "Product1 description", ImageUrl = "product1.jpg" };
            dbContext.Products.Add(product);
            var savedItem = new SavedItem { SavedItemId = Guid.NewGuid().ToString(), UserId = userId, ProductId = productId };
            dbContext.SavedItems.Add(savedItem);
            dbContext.SaveChanges();
            // Act
            var result = await _savedItemService.GetProductFromSavedItems(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            Assert.Equal("Product1", result.Name);
            Assert.Equal(10, result.Price);

        }

        [Fact]
        public async Task GetProductFromSavedItems_WhenCalledWithInvalidProductId_ReturnsNull()
        {
            // Arrange
            int productId = 1;
            string userId = "user123";
            var product = new Product { Id = productId, Name = "Product1", Price = 10, Description = "Product1 description", ImageUrl = "product1.jpg" };
            dbContext.Products.Add(product);
            dbContext.SaveChanges();
            // Act
            var result = await _savedItemService.GetProductFromSavedItems(2);

            // Assert
            Assert.Null(result);

        }
    }
}
