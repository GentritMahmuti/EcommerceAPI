using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface IWishlistService
    {
        Task<List<Product>> GetWishlistContent(string userId);
        Task AddProductToWishlist(string userId, int productId);
        Task RemoveProductFromWishlist(string userId, int productId);
        Task AddToCardFromWishlist(string userId, int productId);
        Task<Product> GetProductFromWishlist(int productId);    }
}
