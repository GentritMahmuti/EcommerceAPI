
using DataAccess.UnitOfWork.IUnitOfWork;
using Domain.Entities;
using EcommerceAPI.Services.IServices;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Services.IServices;

namespace Services.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WishListItem> _logger;
        private readonly ShoppingCardService _shoppingCardService;

        public WishlistService(IUnitOfWork unitOfWork, ILogger<WishListItem> logger, ShoppingCardService shoppingCardService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _shoppingCardService = shoppingCardService;
        }

        public async Task<List<Product>> GetWishlistContent(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("UserId cannot be null or empty");
                throw new ("UserId cannot be null or empty.");
            }

            try
            {
                var wishlist = await _unitOfWork.Repository<WishListItem>().GetByCondition(x => x.UserId == userId).ToListAsync();

                if (!wishlist.Any())
                {
                    return new List<Product>();
                }

                var productsIds = wishlist.Select(x => x.ProductId).ToList();
                var products = await _unitOfWork.Repository<Product>().GetByCondition(x => productsIds.Contains(x.Id)).ToListAsync();
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the wishlist content for user");
                throw new ($"An error happened: '{ex.Message}'");
            }
        }

        public async Task AddProductToWishlist(string userId, int productId)
        {
            try
            {
                var product = await _unitOfWork.Repository<Product>().GetById(x => x.Id == productId).FirstOrDefaultAsync();
                if (product == null)
                {
                    throw new Exception("Product not found");
                }

                var wishlist = await _unitOfWork.Repository<WishListItem>().GetByCondition(w => w.UserId == userId).ToListAsync();

                if (!wishlist.Any(x => x.ProductId == productId))
                {
                    var item = new WishListItem
                    {
                        WishListItemId = Guid.NewGuid().ToString(), 
                        UserId = userId,
                        ProductId = productId,
                        DateCreated = DateTime.Now
                    };

                    _unitOfWork.Repository<WishListItem>().Create(item);
                    _unitOfWork.Complete();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the item in your wishlist ");
                throw new Exception($"An error happened: '{ex.Message}'");
            }
        }

        public async Task RemoveProductFromWishlist(string userId, int productId)
        {
            try
            {
                var product = await _unitOfWork.Repository<Product>().GetById(x => x.Id == productId).FirstOrDefaultAsync();
                if (product == null)
                {
                    throw new Exception("Product not found");
                }

                var wishlist = await _unitOfWork.Repository<WishListItem>().GetByCondition(w => w.UserId == userId).ToListAsync();

                if (wishlist.Any(x => x.ProductId == productId))
                {
                    _unitOfWork.Repository<WishListItem>().Delete(wishlist.FirstOrDefault(x => x.ProductId == productId));
                    _unitOfWork.Complete();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing the item in your wishlist");
                throw new Exception($"An error happened: '{ex.Message}'");
            }
        }

        public async Task AddToCardFromWishlist(string userId, int productId)
        {
            try
            {
                var product = await _unitOfWork.Repository<Product>().GetById(x => x.Id == productId).FirstOrDefaultAsync();
                if (product == null)
                {
                    throw new Exception("Product not found");
                }

                var wishlist = await _unitOfWork.Repository<WishListItem>().GetByCondition(x => x.UserId == userId).ToListAsync();

                if (wishlist.Any(x => x.ProductId == productId))
                {

                    await _shoppingCardService.AddProductToCard(userId, productId, 1);

                    _unitOfWork.Repository<WishListItem>().Delete(wishlist.FirstOrDefault(x => x.ProductId == productId));
                    _unitOfWork.Complete();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the item to the cart");
                throw new Exception("An error occurred while adding the item to the cart");
            }
        }

        public async Task<Product> GetProductFromWishlist(int productId)
        {
            try
            {
                var wishlist = await _unitOfWork.Repository<WishListItem>().GetByCondition(x => x.ProductId == productId).FirstOrDefaultAsync();
                if (wishlist != null)
                {
                    var product = await _unitOfWork.Repository<Product>().GetById(x => x.Id == wishlist.ProductId).FirstOrDefaultAsync();
                    return product;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("There was an error while retrieving the product from your wishlist");
                throw new Exception ($"An error happened: '{ex.Message}'");
            }
        }


    }
}



