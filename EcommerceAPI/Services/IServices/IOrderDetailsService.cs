using EcommerceAPI.Models.DTOs.OrderDetails;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface IOrderDetailsService
    {
        Task CreateOrderDetails(OrderDetailsCreateDto orderDetailsToCreate);
        Task DeleteOrderDetails(int id);
        Task<List<OrderDetails>> GetAllOrderDetails();
       
        Task<OrderDetails> GetOrderDetails(int id);
        Task UpdateOrderDetails(OrderDetails orderDetailsToUpdate);
       
    }
}
