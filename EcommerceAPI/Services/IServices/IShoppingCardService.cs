using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.ShoppingCard;

namespace EcommerceAPI.Services.IServices
{
    public interface IShoppingCardService
    {
        Task AddProductToCard(string userId, int productId, int count);
        Task RemoveProductFromCard(int shoppingCardItemId);
        Task RemoveAllProductsFromCard(string userId);
        Task<ShoppingCardDetails> GetShoppingCardContentForUser(string userId);
        Task IncreaseProductQuantityInShoppingCard(int shoppingCardItemId, int? newQuantity);
        Task DecreaseProductQuantityInShoppingCard(int shoppingCardItemId, int? newQuantity);
        Task CreateOrder(string userId, AddressDetails addressDetails, List<ShoppingCardViewDto> shoppingCardItems, string promoCode);
    }
}
