using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface ISavedItemService
    {
        Task<List<Product>> GetSavedItemsContent(string userId);
        Task AddProductToSavedItems(string userId, int productId);
        Task RemoveProductFromSavedItems(string userId, int productId);
        Task<Product> GetProductFromSavedItems(int productId);
    }
}