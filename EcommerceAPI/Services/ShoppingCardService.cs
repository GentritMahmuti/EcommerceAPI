﻿using Amazon.Runtime.Internal.Util;
using AutoMapper;
using EcommerceAPI.Data;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Helpers;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.ShoppingCard;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Product = EcommerceAPI.Models.Entities.Product;
using Stripe;
using System;
using System.Linq.Expressions;
using System.Text;
using static Amazon.S3.Util.S3EventNotification;
using EcommerceAPI.Models.DTOs.Promotion;
using EcommerceAPI.Models.DTOs.Product;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public class ShoppingCardService : IShoppingCardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ShoppingCardService> _logger;
        private readonly ICacheService _cacheService;
        private List<string> _keys;
        public ShoppingCardService(IUnitOfWork unitOfWork, IMapper mapper, IEmailSender emailSender, ILogger<ShoppingCardService> logger, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailSender = emailSender;
            _logger = logger;
            _cacheService = cacheService;
            _keys = new List<string>();
        }

        private async Task<CartItem> GetCardItem(int itemId)
        {

            var cartItem = await _unitOfWork.Repository<CartItem>()
                .GetById(x => x.CartItemId == itemId)
                .Include("Product")
                .FirstOrDefaultAsync();

            return cartItem;
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
                _unitOfWork.Complete();


                
                var cartItem = await GetCardItem(shoppingCardItem.CartItemId);

                //Check if the data is already in the cache
                var key = $"CartItems_{userId}";
                
                //Store the data in the cache
                var expirationTime = DateTimeOffset.Now.AddDays(1);
                _cacheService.SetDataMember(key, cartItem);
                
                

                
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
                var key = $"CartItems_{userId}";

                // Check if the data is already in the cache
                var usersShoppingCard = _cacheService.GetDataSet<CartItem>(key);

                // If not, then get the data from the database
                if (usersShoppingCard == null)
                {
                    usersShoppingCard = await _unitOfWork.Repository<CartItem>()
                    
                 

                

                                                                        .GetByCondition(x => x.UserId == userId)
                                                                        .Include(x => x.Product)
                                                                        .ToListAsync();
                }

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
                }
                
                var shoppingCardDetails = new ShoppingCardDetails()
                {
                    ShoppingCardItems = shoppingCardList,
                    CardTotal = shoppingCardList.Select(x => x.Total).Sum()
                };
                

                        

                return shoppingCardDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error while trying to get the shopping card content!");
                return new ShoppingCardDetails();
            }
        }




        public async Task RemoveProductFromCard(int shoppingCardItemId, string userId)
        {
            try
            {
                // Retrieve data from the cache


                var cacheKey = $"CartItems_{userId}";

                // Check if the data is already in the cache
                var usersShoppingCard = _cacheService.GetDataSet<CartItem>(cacheKey);
                
                





                // If the data is not found in the cache, retrieve it from the database

                var shoppingCardItem = await _unitOfWork.Repository<CartItem>()
                                                                        .GetById(x => x.CartItemId == shoppingCardItemId)
                                                                        .FirstOrDefaultAsync();


                // Delete data from both cache and database

                _unitOfWork.Repository<CartItem>().Delete(shoppingCardItem);
                _cacheService.RemoveData(cacheKey);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to remove a product to card");
                throw new Exception("An error occurred while removing the item from the cart");

            }
        }

        public async Task RemoveAllProductsFromCard(string userId)
        {
            var shoppingCardItems = _unitOfWork.Repository<CartItem>()
                                                                .GetByCondition(x => x.UserId.Equals(userId))
                                                                .ToList();

            _unitOfWork.Repository<CartItem>().DeleteRange(shoppingCardItems);
            _unitOfWork.Complete();

        }

        public async Task IncreaseProductQuantityInShoppingCard(int shoppingCardItemId, int? newQuantity)
        {

            try
            {
                string cacheKey = string.Format("CartItems_CartItemId_{0}", shoppingCardItemId);


                var shoppingCardItem = await _unitOfWork.Repository<CartItem>()
                                                        .GetById(x => x.CartItemId == shoppingCardItemId)
                                                        .FirstOrDefaultAsync();

                if (newQuantity == null)
                    shoppingCardItem.Count++;
                else
                    shoppingCardItem.Count = (int)newQuantity;

                _unitOfWork.Repository<CartItem>().Update(shoppingCardItem);
                _unitOfWork.Complete();
                _cacheService.SetUpdatedData(cacheKey, shoppingCardItem, DateTimeOffset.Now.AddDays(1));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to add a product to card");
                throw new Exception("An error occurred while adding the item to the cart");

            }
        }
        public async Task DecreaseProductQuantityInShoppingCard(int shoppingCardItemId, int? newQuantity)
        {
            try
            {
                string cacheKey = string.Format("CartItems_CartItemId_{0}", shoppingCardItemId);
                var shoppingCardItem = await _unitOfWork.Repository<CartItem>().GetById(x => x.CartItemId == shoppingCardItemId).FirstOrDefaultAsync();
                if (shoppingCardItem == null)
                {
                    throw new Exception("Cart item not found in the database.");
                }
                if (newQuantity == null)
                    shoppingCardItem.Count--;
                else
                    shoppingCardItem.Count = (int)newQuantity;

                _unitOfWork.Repository<CartItem>().Update(shoppingCardItem);
                _unitOfWork.Complete();
                _cacheService.SetUpdatedData(cacheKey, shoppingCardItem, DateTimeOffset.Now.AddDays(1));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to remove one product to card");
                throw new Exception("An error occurred while removing the item to the cart");

            }
        }

        public async Task CreateOrder(string userId, AddressDetails addressDetails, List<ShoppingCardViewDto> shoppingCardItems, string? promoCode)
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
                product.TotalSold += item.ShopingCardProductCount;

                if (product.Stock == 1 || product.Stock == 10)
                {
                    PublishForLowStock(new LowStockDto { ProductId = product.Id, CurrStock = product.Stock });
                }

                _unitOfWork.Repository<Product>().Update(product);

                var orderDetails = new ProductOrderData
                {
                    OrderDataId = orderId,
                    ProductId = item.ProductId,
                    Count = item.ShopingCardProductCount,
                    Price = item.Total
                };

                orderDetailsList.Add(orderDetails);
                orderCalculatedPrice += item.Total;
            }
            order.OrderPrice = orderCalculatedPrice;
            PromotionDataDto promotionData = new () ;
            promotionData.OrderFinalPrice = orderCalculatedPrice;
            if (!promoCode.IsNullOrEmpty())
            {
                promotionData = await CheckPromoCode(promoCode, orderCalculatedPrice);
                order.PromotionId = promotionData.PromotionId;
            }

            order.OrderFinalPrice = promotionData.OrderFinalPrice;
            


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
                PhoheNumber = addressDetails.PhoheNumber,
                StreetAddress = addressDetails.StreetAddress,
                City = addressDetails.City,
                PostalCode = addressDetails.PostalCode,
            };
            PublishOrderConfirmation(orderConfirmationDto);
        }

        async private Task<PromotionDataDto> CheckPromoCode(string promoCode, double orderTotal)
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
