using AutoMapper;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Helpers;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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


        public async Task<OrderData> GetOrder(string orderId)
        {
            Expression<Func<OrderData, bool>> expression = x => x.OrderId == orderId;
            var orderData = await _unitOfWork.Repository<OrderData>().GetById(expression).FirstOrDefaultAsync();

            return orderData;
        }

        public async Task UpdateOrder(OrderData order)
        {
            var product = await GetOrder(order.OrderId);
            if (product == null)
            {
                throw new NullReferenceException("The orderdata you're trying to update doesn't exist!");
            }
            product.Name = order.Name;

            _unitOfWork.Repository<OrderData>().Update(product);

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
