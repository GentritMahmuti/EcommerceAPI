using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq.Dynamic.Core;

namespace EcommerceAPI.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WishListItem> _logger;

        public WishlistService(IUnitOfWork unitOfWork, ILogger<WishListItem> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<Product>> GetWishlistContent(string userId)
        {
            try
            {
                var wishlist = await _unitOfWork.Repository<WishListItem>().GetByCondition(x => x.UserId == userId).ToListAsync();
                var productsIds = wishlist.Select(x => x.ProductId).ToList();
                var products = await _unitOfWork.Repository<Product>().GetByCondition(x => productsIds.Contains(x.Id)).ToListAsync();
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the wishlist content for user");
                throw new Exception("An error occurred while getting the wishlist content for user");
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
                        WishListItemId = Guid.NewGuid().ToString(), // Add a unique identifier
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
                throw new Exception("An error occurred while adding the item in your wishlist");
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
                throw new Exception("An error occurred while removing the item in your wishlist");
            }
        }
    }
}



