
using Domain.Entities;
using Services.DTOs.Order;

namespace Services.Services.IServices
{
    public interface IOrderService
    {
        Task ChangeOrderStatus(string orderId, string status);
        List<OrderData> GetCustomerOrderHistory(string userId);
        Task<OrderData> GetOrder(string orderId);

        Task CreateOrder(string userId, AddressDetails addressDetails, string? promoCode);

        Task CreateOrderForProduct(string userId, int productId, int count, AddressDetails addressDetails, string? promoCode);

    }
}
