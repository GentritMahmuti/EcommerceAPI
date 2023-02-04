using Domain.Entities;
using Services.DTOs.ShoppingCard;

namespace Services.Services.IServices
{
    public interface IShoppingCardService
    {
        Task AddProductToCard(string userId, int productId, int count);
        Task RemoveProductFromCard(int shoppingCardItemId, string userId);
        Task RemoveAllProductsFromCard(string userId);
        Task<List<CartItem>> GetShoppingCardItems(string userId);
        Task<ShoppingCardDetails> GetShoppingCardContentForUser(string userId);
        Task IncreaseProductQuantityInShoppingCard(int shoppingCardItemId, string userId, int? newQuantity);
        Task DecreaseProductQuantityInShoppingCard(int shoppingCardItemId, string userId, int? newQuantity);
    }
}
