using EcommerceAPI.Models.DTOs.DataOrder;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface IOrderDataService
    {
        Task CreateOrderData(OrderDataCreateDto orderDataToCreate);
        Task DeleteOrderData(int id);
        Task<List<OrderData>> GetAllOrderData();

        Task<OrderData> GetOrderData(int id);
        Task UpdateOrderData(OrderData orderDataToUpdate);
    }
}
