using AutoMapper;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Helpers;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public class OrderService : IOrderService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public OrderData GetOrder(string orderId)
        {
            var orderData = _unitOfWork.Repository<OrderData>().GetByCondition(o => o.OrderId == orderId).SingleOrDefault();
            if (orderData == null)
            {
                throw new Exception("Order not found");
            }
            return orderData;
        }

        public void UpdateOrder(OrderData order)
        {
            _unitOfWork.Repository<OrderData>().Update(order);
            _unitOfWork.Complete();
        }


        public async Task ProcessOrder(List<string> orderIds, string status)
        {
            var ordersToUpdate = await _unitOfWork.Repository<OrderData>()
                                                        .GetByCondition(x => orderIds.Contains(x.OrderId))
                                                        .ToListAsync();
            var carrier = string.Empty;
            if (status == StaticDetails.Shipped)
            {
                carrier = Guid.NewGuid().ToString();
            }

            foreach (var order in ordersToUpdate)
            {
                order.OrderStatus = status;
                order.Carrier = carrier;
            }

            _unitOfWork.Repository<OrderData>().UpdateRange(ordersToUpdate);

            _unitOfWork.Complete();
        }
    }
}
