using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface IOrderService
    {
        Task ProcessOrder(List<string> orderIds, string status);
        Task<OrderData> GetOrder(string orderId);
        Task UpdateOrder(OrderData order);
    }
}
