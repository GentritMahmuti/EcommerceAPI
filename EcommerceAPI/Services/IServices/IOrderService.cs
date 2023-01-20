using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface IOrderService
    {
        Task ProcessOrder(List<string> orderIds, string status);
        OrderData GetOrder(string orderId);
        void UpdateOrder(OrderData order);
    }
}
