
using Persistence.UnitOfWork.IUnitOfWork;
using Domain.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Services.Services
{
    public class SavedItemService : ISavedItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SavedItem> _logger;
        private readonly ShoppingCardService _shoppingCardService;

        public SavedItemService(IUnitOfWork unitOfWork, ILogger<SavedItem> logger, ShoppingCardService shoppingCardService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _shoppingCardService = shoppingCardService;
        }

        public async Task<List<Product>> GetSavedItemsContent(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("UserId cannot be null or empty");
                throw new("UserId cannot be null or empty.");
            }

            try
            {
                var savedItems = await _unitOfWork.Repository<SavedItem>().GetByCondition(x => x.UserId == userId).ToListAsync();

                if (!savedItems.Any())
                {
                    return new List<Product>();
                }

                var productsIds = savedItems.Select(x => x.ProductId).ToList();
                var products = await _unitOfWork.Repository<Product>().GetByCondition(x => productsIds.Contains(x.Id)).ToListAsync();
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the saved items content for user");
                throw new(ex.Message);
            }
        }

        public async Task AddProductToSavedItems(string userId, int productId)
        {
            try
            {
                var product = await _unitOfWork.Repository<Product>().GetById(x => x.Id == productId).FirstOrDefaultAsync();
                if (product == null)
                {
                    throw new Exception("Product not found");
                }

                var savedItems = await _unitOfWork.Repository<SavedItem>().GetByCondition(w => w.UserId == userId).ToListAsync();

                if (!savedItems.Any(x => x.ProductId == productId))
                {
                    var item = new SavedItem
                    {
                        SavedItemId = Guid.NewGuid().ToString(),
                        UserId = userId,
                        ProductId = productId
                    };

                    _unitOfWork.Repository<SavedItem>().Create(item);
                    await _unitOfWork.CompleteAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the item in your saved items ");
                throw new Exception(ex.Message);
            }
        }

        public async Task RemoveProductFromSavedItems(string userId, int productId)
        {
            try
            {
                var product = await _unitOfWork.Repository<Product>().GetById(x => x.Id == productId).FirstOrDefaultAsync();
                if (product == null)
                {
                    throw new Exception("Product not found");
                }

                var savedItems = await _unitOfWork.Repository<SavedItem>().GetByCondition(w => w.UserId == userId).ToListAsync();

                if (savedItems.Any(x => x.ProductId == productId))
                {
                    _unitOfWork.Repository<SavedItem>().Delete(savedItems.FirstOrDefault(x => x.ProductId == productId));
                    await _unitOfWork.CompleteAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing the item in your saved items");
                throw new Exception(ex.Message);
            }
        }

        public async Task<Product> GetProductFromSavedItems(int productId)
        {
            try
            {
                var savedItems = await _unitOfWork.Repository<SavedItem>().GetByCondition(x => x.ProductId == productId).FirstOrDefaultAsync();
                if (savedItems != null)
                {
                    var product = await _unitOfWork.Repository<Product>().GetById(x => x.Id == savedItems.ProductId).FirstOrDefaultAsync();
                    return product;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("There was an error while getting the product from your saved item list. Please try again!");
                throw new Exception(ex.Message);
            }
        }
    }

}