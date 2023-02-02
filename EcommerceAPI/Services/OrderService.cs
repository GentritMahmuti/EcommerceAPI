using AutoMapper;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Helpers;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.DTOs.Promotion;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nest;
using Newtonsoft.Json;
using RabbitMQ.Client;
using StackExchange.Redis;
using System.Linq.Expressions;
using System.Text;

namespace EcommerceAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;
        private readonly IProductService _productService;

        public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger, IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _productService = productService;
        }


        public async Task<OrderData> GetOrder(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                throw new ArgumentException("orderId cannot be null or empty");
            }
            
            var orderData = await _unitOfWork.Repository<OrderData>().GetById(x => x.OrderId == orderId).FirstOrDefaultAsync();

            if (orderData == null)
            {
                throw new NullReferenceException($"Order not found with Id: {orderId}");
            }
            
            return orderData;
        }


        /// <summary>
        /// Gets all orders that a user with a userId has done.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List of orders.</returns>
        public List<OrderData> GetCustomerOrderHistory(string userId)
        {
            var orders = _unitOfWork.Repository<OrderData>().GetByCondition(o => o.UserId == userId).ToList();
            return orders;
        }
        

        /// <summary>
        /// Changes status of the order to the given one and publishes to rabbit queue which then sends information email to user.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task ChangeOrderStatus(string orderId, string status)
        {
            var orderToUpdate = await _unitOfWork.Repository<OrderData>().GetByCondition(x => x.OrderId == orderId).FirstOrDefaultAsync();
           
            if (orderToUpdate == null)
            {
                throw new Exception("No order with the specified Id exists!");
            }

            var carrier = string.Empty;
            if (status == StaticDetails.Approved)
            {
                carrier = Guid.NewGuid().ToString();
            }

            orderToUpdate.OrderStatus = status;
            orderToUpdate.Carrier = carrier;

            var client = await _unitOfWork.Repository<User>().GetByCondition(x => x.Id == orderToUpdate.UserId).FirstOrDefaultAsync();
            
            _unitOfWork.Repository<OrderData>().Update(orderToUpdate);
            await _unitOfWork.CompleteAsync();
            
            var rabbitData = new OrderStatusDto { OrderId = orderToUpdate.OrderId, Name = client.FirsName, Status = status, Email = client.Email };
            PublishRabbit(rabbitData);
        }

        
        /// <summary>
        /// Creates a order from shoppingCard and publishes message to rabbit queue for order confirmation.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressDetails"></param>
        /// <param name="promoCode"></param>
        /// <returns></returns>
        public async Task CreateOrder(string userId, AddressDetails addressDetails, string? promoCode)
        {
            var orderId = Guid.NewGuid().ToString();
            var trackingId = Guid.NewGuid().ToString();
            var orderCalculatedPrice = 0.0;

            var order = new OrderData
            {
                OrderId = orderId,
                OrderDate = DateTime.Now,
                ShippingDate = DateTime.Now.AddDays(7),
                PhoheNumber = addressDetails.PhoheNumber,
                StreetAddress = addressDetails.StreetAddress,
                City = addressDetails.City,
                Country = addressDetails.Country,
                PostalCode = addressDetails.PostalCode,
                Name = addressDetails.Name,
                TrackingId = trackingId,
                UserId = userId
            };

            var shoppingCardItems = await GetShoppingCardItems(userId);

            var orderDetailsList = new List<ProductOrderData>();

            foreach (var item in shoppingCardItems)
            {
                var product = await GetAndUpdateProduct(item);

                var orderDetails = new ProductOrderData
                {
                    OrderDataId = orderId,
                    ProductId = item.ProductId,
                    Count = item.Count,
                    Price = product.Price
                };

                orderDetailsList.Add(orderDetails);
                orderCalculatedPrice += product.Price * orderDetails.Count;
            }
            order.OrderPrice = orderCalculatedPrice;

            PromotionDataDto promotionData = new();
            promotionData.OrderFinalPrice = orderCalculatedPrice;
            if (!promoCode.IsNullOrEmpty())
            {
                promotionData = await CheckPromoCode(promoCode, orderCalculatedPrice);
                order.PromotionId = promotionData.PromotionId;
            }

            order.OrderFinalPrice = promotionData.OrderFinalPrice;

            _unitOfWork.Repository<OrderData>().Create(order);
            _unitOfWork.Repository<CartItem>().DeleteRange(shoppingCardItems);
            _unitOfWork.Repository<ProductOrderData>().CreateRange(orderDetailsList);
                
            await _unitOfWork.CompleteAsync();

            var orderConfirmationDto = new OrderConfirmationDto
            {
                UserName = addressDetails.Name,
                OrderDate = DateTime.Now,
                Price = order.OrderFinalPrice,
                OrderId = orderId,
                Email = addressDetails.Email,
                PhoheNumber = addressDetails.PhoheNumber,
                StreetAddress = addressDetails.StreetAddress,
                City = addressDetails.City,
                PostalCode = addressDetails.PostalCode,
            };
            PublishOrderConfirmation(orderConfirmationDto);
        }


        /// <summary>
        /// Creates an order for a single product and publishes a message to rabbit queue for order confirmation.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <param name="count"></param>
        /// <param name="addressDetails"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public async Task CreateOrderForProduct(string userId, int productId, int count, AddressDetails addressDetails)
        {

            var product = await _unitOfWork.Repository<Product>().GetById(x => x.Id == productId).FirstOrDefaultAsync();
            if (product == null)
            {
                throw new NullReferenceException("The product you're trying to order doesn't exist!");
            }
            var trackingId = Guid.NewGuid().ToString();
            var orderDetailsList = new List<ProductOrderData>();
            var order = new OrderData
            {
                OrderId = Guid.NewGuid().ToString(),
                OrderDate = DateTime.Now,
                ShippingDate = DateTime.Now.AddDays(7),
                OrderPrice = product.Price * count,
                OrderFinalPrice = product.Price * count,
                PhoheNumber = addressDetails.PhoheNumber,
                StreetAddress = addressDetails.StreetAddress,
                City = addressDetails.City,
                Country = addressDetails.Country,
                PostalCode = addressDetails.PostalCode,
                Name = addressDetails.Name,
                TrackingId = trackingId,
                OrderStatus = StaticDetails.Created,
                UserId = userId
            };

            var orderDetails = new ProductOrderData
            {
                OrderDataId = order.OrderId,
                ProductId = productId,
                Count = count,
                Price = product.Price
            };

            orderDetailsList.Add(orderDetails);

            _unitOfWork.Repository<OrderData>().Create(order);

            _unitOfWork.Repository<ProductOrderData>().Create(orderDetails);

            product.Stock -= count;
            product.TotalSold += count;

            if (product.Stock == 0 || product.Stock == 10)
            {
                PublishForLowStock(new LowStockDto { ProductId = product.Id, CurrStock = product.Stock });
            }

            _unitOfWork.Repository<Product>().Update(product);
            _unitOfWork.Complete();
            await _productService.UpdateSomeElastic(product.Id, product.Stock, product.TotalSold);

            await _unitOfWork.CompleteAsync();

            var orderConfirmationDto = new OrderConfirmationDto
            {
                UserName = addressDetails.Name,
                OrderDate = DateTime.Now,
                Price = product.Price * count,
                OrderId = order.OrderId,
                Email = addressDetails.Email,
                PhoheNumber = addressDetails.PhoheNumber,
                StreetAddress = addressDetails.StreetAddress,
                City = addressDetails.City,
                PostalCode = addressDetails.PostalCode,
            };
            PublishOrderConfirmation(orderConfirmationDto);
        }

        /// <summary>
        /// Gets shoppingCard items of a specific client.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List of card items.</returns>
        /// <exception cref="Exception"></exception>
        private async Task<List<CartItem>> GetShoppingCardItems(string userId)
        {
            var shoppingCardItems = await _unitOfWork.Repository<CartItem>().GetByCondition(x => x.UserId == userId).ToListAsync();
            if (!shoppingCardItems.Any())
            {
                throw new Exception("Shopping cart is empty.");
            }
            return shoppingCardItems;
        }

        /// <summary>
        /// Decreases the stock of a product, increases TotalSold.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Updated product.</returns>
        /// <exception cref="Exception"></exception>
        private async Task<Product> GetAndUpdateProduct(CartItem item)
        {
            var product = await _unitOfWork.Repository<Product>().GetById(x => x.Id == item.ProductId).FirstOrDefaultAsync();
            if (product == null)
            {
                throw new Exception("Product not found.");
            }
            if (product.Stock < item.Count)
            {
                throw new Exception("Stock is not sufficient.");
            }
            if (product.Stock == 0 || product.Stock == 10)
            {
                PublishForLowStock(new LowStockDto { ProductId = product.Id, CurrStock = product.Stock });
            }

            product.Stock -= item.Count;
            product.TotalSold += item.Count;
            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.CompleteAsync();
            await _productService.UpdateSomeElastic(product.Id, product.Stock, product.TotalSold);
            return product;
        }

        /// <summary>
        /// Checks if promoCode is valid, if it is valid applies the discount.
        /// </summary>
        /// <param name="promoCode"></param>
        /// <param name="orderTotal"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        private async Task<PromotionDataDto> CheckPromoCode(string promoCode, double orderTotal)
        {
            var promotion = await _unitOfWork.Repository<Promotion>().GetByCondition(x => x.Name.Equals(promoCode)).FirstOrDefaultAsync();
            if (promotion == null)
            {
                throw new NullReferenceException("Promotion code is incorrect.");
            }
            if (!promotion.IsActive())
            {
                throw new NullReferenceException("This promotion code is not active anymore.");
            }
            else
            {
                orderTotal = orderTotal - (orderTotal * promotion.DiscountAmount / 100);
            }

            return new PromotionDataDto { OrderFinalPrice = orderTotal, PromotionId = promotion.Id };



        }

        /// <summary>
        /// Publishes the message to the "orders" queue when status of an order changes.
        /// </summary>
        /// <param name="rabbitData"></param>
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
                _logger.LogInformation($"{nameof(OrderService)} - Data for order status is published to the rabbit!");
            }
        }

        /// <summary>
        /// Publishes the message to the "order-confirmations" queue when an order is done which then sends email to the user who has done that order to notify them that the order has been confirmed.
        /// </summary>
        /// <param name="rabbitData"></param>
        public void PublishOrderConfirmation(OrderConfirmationDto rabbitData)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "order-confirmations",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(rabbitData));

                channel.BasicPublish(exchange: "",
                                     routingKey: "order-confirmations",
                                     basicProperties: null,
                                     body: body);
                _logger.LogInformation("Data for order confirmation is published to the rabbit!");
            }
        }


        /// <summary>
        /// Publishes the message to the "low-stock" queue when a product is in low stock, which then sends email to admin to notify them.
        /// </summary>
        /// <param name="rabbitData"></param>
        public void PublishForLowStock(LowStockDto data)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "low-stock",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

                channel.BasicPublish(exchange: "",
                                     routingKey: "low-stock",
                                     basicProperties: null,
                                     body: body);
                _logger.LogInformation("Data for low-stock is published to the rabbit!");
            }
        }
    }
}
