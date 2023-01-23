using Amazon.Runtime.Internal.Util;
using AutoMapper;
using EcommerceAPI.Data;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Helpers;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.ShoppingCard;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using static Amazon.S3.Util.S3EventNotification;

namespace EcommerceAPI.Services
{
    public class ShoppingCardService : IShoppingCardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ShoppingCardService> _logger;
        private readonly ICacheService _cacheService;
        public ShoppingCardService(IUnitOfWork unitOfWork, IMapper mapper, IEmailSender emailSender, ILogger<ShoppingCardService> logger, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailSender = emailSender;
            _logger = logger;
            _cacheService = cacheService;
        }


        public async Task AddProductToCard(string userId, int productId, int count)
        {
            try
            {
                var shoppingCardItem = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Count = count
                };

                _unitOfWork.Repository<CartItem>().Create(shoppingCardItem);
                _unitOfWork.Complete(); // add this line

                //Check if the data is already in the cache
                var key = $"UserId_{userId}_ProductId_{productId}";
                var itemInCache = _cacheService.GetData<CartItem>(key);
                if (itemInCache == null)
                {
                    //Store the data in the cache
                    var expirationTime = DateTimeOffset.Now.AddDays(1);
                    _cacheService.SetData(key, shoppingCardItem, expirationTime);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to add a product to card");
            }
        }

        public async Task<ShoppingCardDetails> GetShoppingCardContentForUser(string userId)
        {
            try
            {
                // Log the key
                var key = $"UserId_{userId}";
                _logger.LogInformation("The key passed to cache service is: " + key);

                // Check if the data is already in the cache
                ShoppingCardDetails shoppingCardDetails = _cacheService.GetData<ShoppingCardDetails>(key);

                // Check the connection
                _cacheService.SetData("test", "test value", DateTimeOffset.Now.AddDays(1));
                var testValue = _cacheService.GetData<string>("test");
                _logger.LogInformation("The value retrieved from cache is: " + testValue);

                // If not, then get the data from the database
                if (shoppingCardDetails == null)
                {
                    var usersShoppingCard = await _unitOfWork.Repository<CartItem>()
                                                                        .GetByCondition(x => x.UserId == userId)
                                                                        .Include(x => x.Product)
                                                                        .ToListAsync();

                    var shoppingCardList = new List<ShoppingCardViewDto>();

                    foreach (CartItem item in usersShoppingCard)
                    {
                        var currentProduct = item.Product;

                        var model = new ShoppingCardViewDto
                        {
                            ShoppingCardItemId = item.CartItemId,
                            ProductId = item.ProductId,
                            ProductImage = currentProduct.ImageUrl,
                            ProductDescription = currentProduct.Description,
                            ProductName = currentProduct.Name,
                            ProductPrice = currentProduct.Price,
                            ShopingCardProductCount = item.Count,
                            Total = currentProduct.Price * item.Count
                        };

                        shoppingCardList.Add(model);
                        shoppingCardDetails = new ShoppingCardDetails()
                        {
                            ShoppingCardItems = shoppingCardList,
                            CardTotal = shoppingCardList.Select(x => x.Total).Sum()
                        };

                        // Store the data in the cache
                        _cacheService.SetData<ShoppingCardDetails>(userId, shoppingCardDetails, DateTimeOffset.Now.AddDays(1));
                    }
                }
                return shoppingCardDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error while tryng to get the shopping card content!");
                return new ShoppingCardDetails();
            }
        }

        public async Task RemoveProductFromCard(int shoppingCardItemId)
        {
            try
            {
                // Retrieve data from the cache
                string cacheKey = string.Format("CartItem_{0}", shoppingCardItemId);
                var shoppingCardItem = _cacheService.GetData<CartItem>(cacheKey);

                // If the data is not found in the cache, retrieve it from the database
                if (shoppingCardItem == null)
                {
                    shoppingCardItem = await _unitOfWork.Repository<CartItem>()
                                                                        .GetById(x => x.CartItemId == shoppingCardItemId)
                                                                        .FirstOrDefaultAsync();
                }

                // Delete data from both cache and database
                _cacheService.RemoveData(cacheKey);
                _unitOfWork.Repository<CartItem>().Delete(shoppingCardItem);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to remove a product to card");
                throw new Exception("An error occurred while removing the item from the cart");

            }
        }

        public async Task Plus(int shoppingCardItemId, int? newQuantity)
        {
            try
            {
                var shoppingCardItem = _cacheService.GetData<CartItem>(shoppingCardItemId.ToString());
                if (shoppingCardItem == null)
                {
                    shoppingCardItem = await _unitOfWork.Repository<CartItem>()
                                                            .GetById(x => x.CartItemId == shoppingCardItemId)
                                                            .FirstOrDefaultAsync();
                }

                if (newQuantity == null)
                    shoppingCardItem.Count++;
                else
                    shoppingCardItem.Count = (int)newQuantity;

                _unitOfWork.Repository<CartItem>().Update(shoppingCardItem);
                _unitOfWork.Complete();
                _cacheService.SetUpdatedData(shoppingCardItemId.ToString(), shoppingCardItem, DateTimeOffset.Now.AddDays(1));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to add a product to card");
                throw new Exception("An error occurred while adding the item to the cart");

            }
        }
        public async Task Minus(int shoppingCardItemId, int? newQuantity)
        {
            try
            {
                var cacheKey = $"cart-item-{shoppingCardItemId}";
                var shoppingCardItem = _cacheService.GetData<CartItem>(cacheKey);

                if (shoppingCardItem == null)
                {
                    var itemInDb = await _unitOfWork.Repository<CartItem>().GetById(x => x.CartItemId == shoppingCardItemId).FirstOrDefaultAsync();
                    if (itemInDb == null)
                    {
                        throw new Exception("Cart item not found in the database.");
                    }
                    shoppingCardItem = itemInDb;
                }

                if (newQuantity == null)
                    shoppingCardItem.Count--;
                else
                    shoppingCardItem.Count = (int)newQuantity;

                _unitOfWork.Repository<CartItem>().Update(shoppingCardItem);
                _unitOfWork.Complete();
                _cacheService.RemoveData(cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to remove one product to card");
                throw new Exception("An error occurred while removing the item to the cart");

            }
        }

        public async Task CreateOrder(AddressDetails addressDetails, List<ShoppingCardViewDto> shoppingCardItems)
        {
            var orderId = Guid.NewGuid().ToString();
            var trackingId = Guid.NewGuid().ToString();
            var orderTotal = 0L;

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
                TrackingId = trackingId
            };

            var orderDetailsList = new List<ProductOrderData>();

            foreach (ShoppingCardViewDto item in shoppingCardItems)
            {
                var product = await _unitOfWork.Repository<Product>().GetById(x => x.Id == item.ProductId).FirstOrDefaultAsync();
                if (product == null)
                {
                    throw new Exception("Product not found.");
                }

                if (product.Stock < item.ShopingCardProductCount)
                {
                    throw new Exception("Stock is not sufficient.");
                }

                product.Stock -= item.ShopingCardProductCount;
                _unitOfWork.Repository<Product>().Update(product);

                var orderDetails = new ProductOrderData
                {
                    OrderDataId = orderId,
                    ProductId = item.ProductId,
                    Count = item.ShopingCardProductCount,
                    Price = item.Total
                };

                orderDetailsList.Add(orderDetails);
                orderTotal += (long)item.Total;
            }
            order.OrderTotal = orderTotal;

            var shoppingCardItemIdsToRemove = shoppingCardItems.Select(x => x.ShoppingCardItemId).ToList();
            var shoppingCardItemsToRemove = await _unitOfWork.Repository<CartItem>()
                                                     .GetByCondition(x => shoppingCardItemIdsToRemove.Contains(x.CartItemId))
                                                     .ToListAsync();

            _unitOfWork.Repository<OrderData>().Create(order);
            _unitOfWork.Repository<CartItem>().DeleteRange(shoppingCardItemsToRemove);
            _unitOfWork.Repository<ProductOrderData>().CreateRange(orderDetailsList);

            _unitOfWork.Complete();

            double totalPrice = 0;
            totalPrice = shoppingCardItems.Select(x => x.Total).Sum();

            var orderConfirmationDto = new OrderConfirmationDto
            {
                UserName = addressDetails.Name,
                OrderDate = DateTime.Now,
                Price = totalPrice,
                OrderId = orderId,
                Email = addressDetails.Email,
            };
            PublishOrderConfirmation(orderConfirmationDto);
        }
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

    }
}
