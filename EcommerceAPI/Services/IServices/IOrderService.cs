using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface IOrderService
    {
        Task ChangeOrderStatus(string orderId, string status);
        List<OrderData> GetCustomerOrderHistory(string userId);
        Task<OrderData> GetOrder(string orderId);
        Task UpdateOrder(OrderData order);
    }
}
