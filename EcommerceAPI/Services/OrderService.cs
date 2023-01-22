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
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
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

        public async Task ProcessOrder(string orderId, string status)
        {
            var orderToUpdate = await _unitOfWork.Repository<OrderData>()
                                                        .GetByCondition(x => x.OrderId == orderId)
                                                        .FirstOrDefaultAsync();
           
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

            var client = await _unitOfWork.Repository<User>()
                                                   .GetByCondition(x => x.Id == orderToUpdate.UserId)
                                                   .FirstOrDefaultAsync();
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
