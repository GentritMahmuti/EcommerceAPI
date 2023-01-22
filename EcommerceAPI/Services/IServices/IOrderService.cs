using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface IOrderService
    {
        Task ProcessOrder(string orderId, string status);
        Task<OrderData> GetOrder(string orderId);
        Task UpdateOrder(OrderData order);
    }
}
