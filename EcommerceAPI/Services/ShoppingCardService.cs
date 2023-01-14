using AutoMapper;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Helpers;
using EcommerceAPI.Models.DTOs.OrderDetails;
using EcommerceAPI.Models.DTOs.ShoppingCard;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

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
            var shoppingCardItem = new ShoppingCard
            {
                UserId = userId,
                ProductId = productId,
                Count = count
            };

            _unitOfWork.Repository<ShoppingCard>().Create(shoppingCardItem);
            _unitOfWork.Complete();
        }

        public async Task<ShoppingCardDetails> GetShoppingCardContentForUser(string userId)
        {
            var usersShoppingCard = await _unitOfWork.Repository<ShoppingCard>()
                                                                    .GetByCondition(x => x.UserId == userId)
                                                                    .Include(x => x.Product)
                                                                    .ToListAsync();

            var shoppingCardList = new List<ShoppingCardViewDto>();

            foreach (ShoppingCard item in usersShoppingCard)
            {
                var currentProduct = item.Product;

                var calculatedPrice = HelperMethods.GetPriceByQuantity(item.Count, currentProduct.Price, currentProduct.Price50, currentProduct.Price100);

                var model = new ShoppingCardViewDto
                {
                    ShoppingCardItemId = item.Id,
                    ProductId = item.ProductId,
                    ProductImage = currentProduct.ImageUrl,
                    ProductDescription = currentProduct.Description,
                    ProductName = currentProduct.Title,
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
            var shoppingCardItem = await _unitOfWork.Repository<ShoppingCard>()
                                                                .GetById(x => x.Id == shoppingCardItemId)
                                                                .FirstOrDefaultAsync();

            if (newQuantity == null)
                shoppingCardItem.Count++;
            else
                shoppingCardItem.Count = (int)newQuantity;

            _unitOfWork.Repository<ShoppingCard>().Update(shoppingCardItem);
            _unitOfWork.Complete();
        }

        public async Task Minus(int shoppingCardItemId, int? newQuantity)
        {
            var shoppingCardItem = await _unitOfWork.Repository<ShoppingCard>()
                                                                .GetById(x => x.Id == shoppingCardItemId)
                                                                .FirstOrDefaultAsync();

            if (newQuantity == null)
                shoppingCardItem.Count--;
            else
                shoppingCardItem.Count = (int)newQuantity;

            _unitOfWork.Repository<ShoppingCard>().Update(shoppingCardItem);
            _unitOfWork.Complete();
        }

        public async Task CreateOrder(AddressDetails addressDetails, List<ShoppingCardViewDto> shoppingCardItems)
        {
            var orders = new List<OrderData>();
            List<int>? shoppingCardItemIdsToRemove = new();

            var trackingId = Guid.NewGuid().ToString();

            var orderDetailsList = new List<OrderDetails>();

            foreach (ShoppingCardViewDto item in shoppingCardItems)
            {
                var order = new OrderData
                {
                    OrderDate = DateTime.Now,
                    ShippingDate = DateTime.Now.AddDays(7),
                    OrderTotal = item.Total,
                    PhoheNumber = addressDetails.PhoheNumber,
                    StreetAddress = addressDetails.StreetAddress,
                    City = addressDetails.City,
                    Country = addressDetails.Country,
                    PostalCode = addressDetails.PostalCode,
                    Name = addressDetails.Name,
                    TrackingId = trackingId
                };

                var orderDetails = new OrderDetails
                {
                    OrderDataId = order.OrderId,
                    ProductId = item.ProductId,
                    Count = item.ShopingCardProductCount,
                    Price = item.Total
                };

                orderDetailsList.Add(orderDetails);

                orders.Add(order);

                shoppingCardItemIdsToRemove.Add(item.ShoppingCardItemId);

            }

            var shoppingCardItemsToRemove = await _unitOfWork.Repository<ShoppingCard>()
                                                                          .GetByCondition(x => shoppingCardItemIdsToRemove.Contains(x.Id))
                                                                          .ToListAsync();

            _unitOfWork.Repository<OrderData>().CreateRange(orders);

            _unitOfWork.Repository<ShoppingCard>().DeleteRange(shoppingCardItemsToRemove);

            _unitOfWork.Repository<OrderDetails>().CreateRange(orderDetailsList);

            _unitOfWork.Complete();

            var pathToFile = "Templates/order_confirmation.html";

            string htmlBody = "";
            using (StreamReader streamReader = System.IO.File.OpenText(pathToFile))
            {
                htmlBody = streamReader.ReadToEnd();
            }

            double totalPrice = 0;
            shoppingCardItems.ForEach(x => totalPrice += x.ProductPrice);

            var orderIds = orders.Select(x => x.OrderId).ToList();

            //var totalPrice = shoppingCardItems.Select(x => x.ProductPrice).Sum();
            var orderConfirmationDto = new OrderConirmationDto
            {
                UserName = "LifeUser",
                OrderDate = DateTime.Now,
                Price = totalPrice,
                OrderId = string.Join(",", orderIds)
            };

            var myData = new[] { "LifeUser", DateTime.Now.ToString(), totalPrice.ToString(), string.Join(",", orderIds) };

            var content = string.Format(htmlBody, myData);

            await _emailSender.SendEmailAsync(addressDetails.Email, "OrderConfirmation", content);
        }
    }
}
