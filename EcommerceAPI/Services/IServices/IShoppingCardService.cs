using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.ShoppingCard;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface IShoppingCardService
    {
        Task AddProductToCard(string userId, int productId, int count);
        Task RemoveProductFromCard(int shoppingCardItemId, string userId);
        Task RemoveAllProductsFromCard(string userId);
        Task<ShoppingCardDetails> GetShoppingCardContentForUser(string userId);
        Task IncreaseProductQuantityInShoppingCard(int shoppingCardItemId, string userId, int? newQuantity);
        Task DecreaseProductQuantityInShoppingCard(int shoppingCardItemId, string userId, int? newQuantity);
        Task CreateOrder(string userId, AddressDetails addressDetails, List<ShoppingCardViewDto> shoppingCardItems, string promoCode);
    }
}
