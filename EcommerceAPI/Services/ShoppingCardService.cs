using AutoMapper;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Helpers;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.ShoppingCard;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Stripe;
using System;
using System.Linq.Expressions;
using System.Text;

namespace EcommerceAPI.Services
{
    public class ShoppingCardService : IShoppingCardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ShoppingCardService> _logger;
        public ShoppingCardService(IUnitOfWork unitOfWork, IMapper mapper, IEmailSender emailSender, ILogger<ShoppingCardService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailSender = emailSender;
            _logger = logger;
        }


        public async Task AddProductToCard(string userId, int productId, int count)
        {
            
            var shoppingCardItem = new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Count = count
            };
            
            _unitOfWork.Repository<CartItem>().Create(shoppingCardItem);
            _unitOfWork.Complete();
        }

        public async Task<ShoppingCardDetails> GetShoppingCardContentForUser(string userId)
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
            }

            var shoppingCardDetails = new ShoppingCardDetails()
            {
                ShoppingCardItems = shoppingCardList,
                CardTotal = shoppingCardList.Select(x => x.Total).Sum()
            };

            return shoppingCardDetails;
        }

        public async Task RemoveProductFromCard(int shoppingCardItemId)
        {
            var shoppingCardItem = await _unitOfWork.Repository<CartItem>()
                                                                .GetById(x => x.CartItemId == shoppingCardItemId)
                                                                .FirstOrDefaultAsync();

            _unitOfWork.Repository<CartItem>().Delete(shoppingCardItem);
            _unitOfWork.Complete();
            
        }

        public async Task Plus(int shoppingCardItemId, int? newQuantity)
        {
            var shoppingCardItem = await _unitOfWork.Repository<CartItem>()
                                                                .GetById(x => x.CartItemId == shoppingCardItemId)
                                                                .FirstOrDefaultAsync();

            if (newQuantity == null)
                shoppingCardItem.Count++;
            else
                shoppingCardItem.Count = (int)newQuantity;

            _unitOfWork.Repository<CartItem>().Update(shoppingCardItem);
            _unitOfWork.Complete();
        }

        public async Task Minus(int shoppingCardItemId, int? newQuantity)
        {
            var shoppingCardItem = await _unitOfWork.Repository<CartItem>()
                                                                .GetById(x => x.CartItemId == shoppingCardItemId)
                                                                .FirstOrDefaultAsync();

            if (newQuantity == null)
                shoppingCardItem.Count--;
            else
                shoppingCardItem.Count = (int)newQuantity;

            _unitOfWork.Repository<CartItem>().Update(shoppingCardItem);
            _unitOfWork.Complete();
        }

        public async Task CreateOrder(AddressDetails addressDetails, List<ShoppingCardViewDto> shoppingCardItems, string promoCode)
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
                TrackingId = trackingId
            };

            var orderDetailsList = new List<ProductOrderData>();

            foreach (ShoppingCardViewDto item in shoppingCardItems)
            {
                var product = await _unitOfWork.Repository<Models.Entities.Product>().GetById(x => x.Id == item.ProductId).FirstOrDefaultAsync();
                if (product == null)
                {
                    throw new Exception("Product not found.");
                }

                if (product.Stock < item.ShopingCardProductCount)
                {
                    throw new Exception("Stock is not sufficient.");
                }

                product.Stock -= item.ShopingCardProductCount;
                _unitOfWork.Repository<Models.Entities.Product>().Update(product);
                
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
            (int PromotionId, double orderFinalPrice) promotionData;
            promotionData.orderFinalPrice = orderCalculatedPrice;
            promotionData.PromotionId = 0;
            if (!promoCode.IsNullOrEmpty())
            {
                promotionData = await CheckPromoCode(promoCode, orderCalculatedPrice);
                order.PromotionId = promotionData.PromotionId;
            }
            order.OrderFinalPrice = promotionData.orderFinalPrice;
            

            var shoppingCardItemIdsToRemove = shoppingCardItems.Select(x => x.ShoppingCardItemId).ToList();
            var shoppingCardItemsToRemove = await _unitOfWork.Repository<CartItem>()
                                                     .GetByCondition(x => shoppingCardItemIdsToRemove.Contains(x.CartItemId))
                                                     .ToListAsync();

            _unitOfWork.Repository<OrderData>().Create(order);
            _unitOfWork.Repository<CartItem>().DeleteRange(shoppingCardItemsToRemove);
            _unitOfWork.Repository<ProductOrderData>().CreateRange(orderDetailsList);

            _unitOfWork.Complete();

            //double totalPrice = 0;
            //totalPrice = shoppingCardItems.Select(x => x.Total).Sum();

            //var orderConfirmationDto = new OrderConfirmationDto
            //{
            //    UserName = addressDetails.Name,
            //    OrderDate = DateTime.Now,
            //    Price = totalPrice,
            //    OrderId = orderId,
            //    Email = addressDetails.Email,
            //};
            //PublishOrderConfirmation(orderConfirmationDto);
        }

        async private Task<(int PromotionId, double orderTotal)> CheckPromoCode(string promoCode, double orderTotal)
        {
            var promotion = await _unitOfWork.Repository<Promotion>().GetByCondition(x=>x.Name.Equals(promoCode)).FirstOrDefaultAsync();
            if(promotion == null)
            {  
                throw new NullReferenceException("Promotion code is incorrect.");
            }
            if(!promotion.IsActive())
            {
                throw new NullReferenceException("This promotion code is not active anymore.");
            }
            else
            {
                orderTotal = orderTotal - (orderTotal * promotion.DiscountAmount / 100);
            }
            return (promotion.Id, orderTotal);
           
            
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
