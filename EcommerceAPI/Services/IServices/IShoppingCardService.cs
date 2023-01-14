using EcommerceAPI.Models.DTOs.OrderDetails;
using EcommerceAPI.Models.DTOs.ShoppingCard;

namespace EcommerceAPI.Services.IServices
{
    public interface IShoppingCardService
    {
        Task AddProductToCard(string userId, int productId, int count);
        Task<ShoppingCardDetails> GetShoppingCardContentForUser(string userId);
        Task Plus(int shoppingCardItemId, int? newQuantity);
        Task Minus(int shoppingCardItemId, int? newQuantity);
        Task CreateOrder(AddressDetails addressDetails, List<ShoppingCardViewDto> shoppingCardItems);
    }
}
