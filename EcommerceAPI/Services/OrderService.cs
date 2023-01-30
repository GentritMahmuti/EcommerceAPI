using AutoMapper;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Helpers;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Linq.Expressions;
using System.Text;

namespace EcommerceAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<OrderData> GetOrder(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                _logger.LogError("GetOrder - orderId is null or empty");
                throw new ArgumentException("orderId cannot be null or empty");
            }
            
            Expression<Func<OrderData, bool>> expression = x => x.OrderId == orderId;
            var orderData = await _unitOfWork.Repository<OrderData>().GetById(expression).FirstOrDefaultAsync();

            if (orderData == null)
            {
                _logger.LogError($"GetOrder - Order not found with Id: {orderId}");
                throw new NullReferenceException($"Order not found with Id: {orderId}");
            }
            
            return orderData;
        }

        public List<OrderData> GetCustomerOrderHistory(string userId)
        {
            var orders = _unitOfWork.Repository<OrderData>().GetByCondition(o => o.UserId == userId).ToList();
            return orders;
        }
        
        public async Task UpdateOrder(OrderData order)
        {
            if (order == null)
            {
                _logger.LogError("Order data input is null");
                throw new ArgumentNullException("Order data input cannot be null");
            }

            var existingOrder = await GetOrder(order.OrderId);
            if (existingOrder == null)
            {
                _logger.LogError("The orderdata you're trying to update doesn't exist! OrderId: {0}", order.OrderId);
                throw new NullReferenceException("The orderdata you're trying to update doesn't exist!");
            }
            existingOrder.Name = order.Name;

            _unitOfWork.Repository<OrderData>().Update(existingOrder);
            _unitOfWork.Complete();
        }

        public async Task ChangeOrderStatus(string orderId, string status)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                _logger.LogError("ChangeOrderStatus - orderId is null or empty");
                throw new ArgumentException("orderId cannot be null or empty");
            }

            if (string.IsNullOrEmpty(status))
            {
                _logger.LogError("ChangeOrderStatus - status is null or empty");
                throw new ArgumentException("status cannot be null or empty");
            }
            
            var orderToUpdate = await _unitOfWork.Repository<OrderData>().GetByCondition(x => x.OrderId == orderId).FirstOrDefaultAsync();
           
            if (orderToUpdate == null)
            {
                throw new Exception("No order with the specified Id exists!");
            }

            var carrier = string.Empty;
            if (status == StaticDetails.Shipped)
            {
                carrier = Guid.NewGuid().ToString();
            }

            orderToUpdate.OrderStatus = status;
            orderToUpdate.Carrier = carrier;

            var client = await _unitOfWork.Repository<User>().GetByCondition(x => x.Id == orderToUpdate.UserId).FirstOrDefaultAsync();
            var clientName = client.FirsName;
            var clientEmail = client.Email;
            
            _unitOfWork.Repository<OrderData>().Update(orderToUpdate);
            _unitOfWork.Complete();
            
            var rabbitData = new OrderStatusDto { OrderId = orderToUpdate.OrderId, Name = clientName, Status = status, Email = clientEmail};
            PublishRabbit(rabbitData);
        }

        public void PublishRabbit(OrderStatusDto rabbitData)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "orders",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(rabbitData));

                channel.BasicPublish(exchange: "",
                                     routingKey: "orders",
                                     basicProperties: null,
                                     body: body);
                _logger.LogInformation("Data for order status is published to the rabbit!");
            }
        }
    }
}
