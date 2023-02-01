using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface IOrderService
    {
        Task ChangeOrderStatus(string orderId, string status);
        List<OrderData> GetCustomerOrderHistory(string userId);
        Task<OrderData> GetOrder(string orderId);
        Task UpdateOrder(OrderData order);

        Task CreateOrder(string userId, AddressDetails addressDetails, string? promoCode);

        Task CreateOrderForProduct(string userId, int productId, int count, AddressDetails addressDetails);

    }
}
