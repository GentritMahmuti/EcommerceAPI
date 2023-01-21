using AutoMapper;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Helpers;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.ShoppingCard;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System;

namespace EcommerceAPI.Services
{
    public class ShoppingCardService : IShoppingCardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;

        public ShoppingCardService(IUnitOfWork unitOfWork, IMapper mapper, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailSender = emailSender;
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

                var calculatedPrice = HelperMethods.GetPriceByQuantity(item.Count, currentProduct.Price);

                var model = new ShoppingCardViewDto
                {
                    ShoppingCardItemId = item.CartItemId,
                    ProductId = item.ProductId,
                    ProductImage = currentProduct.ImageUrl,
                    ProductDescription = currentProduct.Description,
                    ProductName = currentProduct.Name,
                    ProductPrice = calculatedPrice,
                    ShopingCardProductCount = item.Count,
                    Total = calculatedPrice * item.Count
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

            //var pathToFile = "Templates/order_confirmation.html";

            //string htmlBody = "";
            //using (StreamReader streamReader = System.IO.File.OpenText(pathToFile))
            //{
            //    htmlBody = streamReader.ReadToEnd();
            //}

            //double totalPrice = 0;
            //shoppingCardItems.ForEach(x => totalPrice += x.ProductPrice);

            //var orderIds = orders.Select(x => x.OrderId).ToList();

            ////var totalPrice = shoppingCardItems.Select(x => x.ProductPrice).Sum();
            //var orderConfirmationDto = new OrderConirmationDto
            //{
            //    UserName = "LifeUser",
            //    OrderDate = DateTime.Now,
            //    Price = totalPrice,
            //    OrderId = string.Join(",", orderIds)
            //};

            //var myData = new[] { "LifeUser", DateTime.Now.ToString(), totalPrice.ToString(), string.Join(",", orderIds) };

            //var content = string.Format(htmlBody, myData);

            //await _emailSender.SendEmailAsync(addressDetails.Email, "OrderConfirmation", content);
        }


    }
}
