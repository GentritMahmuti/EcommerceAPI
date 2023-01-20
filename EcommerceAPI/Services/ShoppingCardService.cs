//using AutoMapper;
//using EcommerceAPI.Data.UnitOfWork;
//using EcommerceAPI.Helpers;
//using EcommerceAPI.Models.DTOs.Order;
//using EcommerceAPI.Models.DTOs.ShoppingCard;
//using EcommerceAPI.Models.Entities;
//using EcommerceAPI.Services.IServices;
//using Microsoft.AspNetCore.Identity.UI.Services;
//using Microsoft.EntityFrameworkCore;
//using System;

//namespace EcommerceAPI.Services
//{
//    public class ShoppingCardService : IShoppingCardService
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//        private readonly IEmailSender _emailSender;
//        private readonly ICacheService _cacheService;
//        private readonly ILogger<ShoppingCardService> _logger;
//        public ShoppingCardService(IUnitOfWork unitOfWork, IMapper mapper, IEmailSender emailSender, ICacheService cacheService, ILogger<ShoppingCardService> logger)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//            _emailSender = emailSender;
//            _cacheService = cacheService;
//            _logger = logger;
//        }


//        public async Task AddProductToCard(string userId, int productId, int count)
//        {
//            var shoppingCardItem = new ShoppingCard
//            {
//                UserId = userId,
//                ProductId = productId,
//                Count = count
//            };

//            _unitOfWork.Repository<ShoppingCard>().Create(shoppingCardItem);
//            _unitOfWork.Complete();
//        }

//        public async Task<ShoppingCardDetails> GetShoppingCardContentForUser(string userId)
//        {
//            var cacheKey = $"shoppingCard-{userId}";
//            List<ShoppingCardViewDto> shoppingCardList = _cacheService.GetUpdatedData<List<ShoppingCardViewDto>>(cacheKey);
//            if (shoppingCardList == null)
//            {
//                try
//                {
//                    var usersShoppingCard = await _unitOfWork.Repository<ShoppingCard>()
//                                                                                .GetByCondition(x => x.UserId == userId)
//                                                                                .Include(x => x.Product)
//                                                                                .ToListAsync();
//                    shoppingCardList = new List<ShoppingCardViewDto>();

//                    foreach (ShoppingCard item in usersShoppingCard)
//                    {
//                        var currentProduct = item.Product;

//                        var calculatedPrice = HelperMethods.GetPriceByQuantity(item.Count, currentProduct.Price);

//                        var model = new ShoppingCardViewDto
//                        {
//                            ShoppingCardItemId = item.Id,
//                            ProductId = item.ProductId,
//                            ProductImage = currentProduct.ImageUrl,
//                            ProductDescription = currentProduct.Description,
//                            ProductName = currentProduct.Title,
//                            ProductPrice = calculatedPrice,
//                            ShopingCardProductCount = item.Count,
//                            Total = calculatedPrice * item.Count
//                        };

//                        shoppingCardList.Add(model);
//                    }
//                    _cacheService.SetData(cacheKey, shoppingCardList, DateTimeOffset.Now.AddMinutes(30));
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "An error occurred while getting shopping card content for user.");
//                }
//            }
//            var shoppingCardDetails = new ShoppingCardDetails()
//            {
//                ShoppingCardItems = shoppingCardList,
//                CardTotal = shoppingCardList.Select(x => x.Total).Sum()
//            };

//            return shoppingCardDetails;
//        }


//        public async Task Plus(int shoppingCardItemId, int? newQuantity)
//        {
//            var cacheKey = $"shoppingCard-{shoppingCardItemId}";
//            var shoppingCardItem = _cacheService.GetUpdatedData<ShoppingCard>(cacheKey);
//            try
//            {
//                if (shoppingCardItem == null)
//                {
//                    shoppingCardItem = await _unitOfWork.Repository<ShoppingCard>()
//                                                                        .GetById(x => x.Id == shoppingCardItemId)
//                                                                        .FirstOrDefaultAsync();
//                }
//                if (newQuantity == null)
//                    shoppingCardItem.Count++;
//                else
//                    shoppingCardItem.Count = (int)newQuantity;

//                _unitOfWork.Repository<ShoppingCard>().Update(shoppingCardItem);
//                _unitOfWork.Complete();
//                _cacheService.SetUpdatedData(cacheKey, shoppingCardItem, DateTimeOffset.Now.AddMinutes(30));
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while updating shopping card item.");
//            }
//        }


//        public async Task Minus(int shoppingCardItemId, int? newQuantity)
//        {
//            var cacheKey = $"shoppingCard-{shoppingCardItemId}";
//            var shoppingCardItem = _cacheService.GetUpdatedData<ShoppingCard>(cacheKey);
//            try
//            {
//                if (shoppingCardItem == null)
//                {
//                    shoppingCardItem = await _unitOfWork.Repository<ShoppingCard>()
//                                                                            .GetById(x => x.Id == shoppingCardItemId)
//                                                                            .FirstOrDefaultAsync();
//                }
//                if (newQuantity == null)
//                    shoppingCardItem.Count--;
//                else
//                    shoppingCardItem.Count = (int)newQuantity;

//                _unitOfWork.Repository<ShoppingCard>().Update(shoppingCardItem);
//                _unitOfWork.Complete();
//                _cacheService.SetUpdatedData(cacheKey, shoppingCardItem, DateTimeOffset.Now.AddMinutes(30));
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while updating shopping card item.");
//            }
//        }


//        public async Task CreateOrder(AddressDetails addressDetails, List<ShoppingCardViewDto> shoppingCardItems)
//        {
//            var orders = new List<OrderData>();
//            List<int>? shoppingCardItemIdsToRemove = new();

//            var trackingId = Guid.NewGuid().ToString();

//            var orderDetailsList = new List<OrderDetails>();

//            try
//            {
//                foreach (ShoppingCardViewDto item in shoppingCardItems)
//                {
//                    var order = new OrderData
//                    {
//                        OrderId = Guid.NewGuid().ToString(),
//                        OrderDate = DateTime.Now,
//                        ShippingDate = DateTime.Now.AddDays(7),
//                        OrderTotal = (long)item.Total,
//                        PhoheNumber = addressDetails.PhoheNumber,
//                        StreetAddress = addressDetails.StreetAddress,
//                        City = addressDetails.City,
//                        Country = addressDetails.Country,
//                        PostalCode = addressDetails.PostalCode,
//                        Name = addressDetails.Name,
//                        TrackingId = trackingId
//                    };

//                    var orderDetails = new OrderDetails
//                    {
//                        OrderDataId = order.OrderId,
//                        ProductId = item.ProductId,
//                        Count = item.ShopingCardProductCount,
//                        Price = item.Total
//                    };

//                    // Add the data to cache
//                    _cacheService.SetData(orderDetails.OrderDataId, orderDetails, DateTimeOffset.Now.AddDays(1));

//                    orderDetailsList.Add(orderDetails);

//                    orders.Add(order);

//                    shoppingCardItemIdsToRemove.Add(item.ShoppingCardItemId);

//                }

//                var shoppingCardItemsToRemove = await _unitOfWork.Repository<ShoppingCard>()
//                                                     .GetByCondition(x => shoppingCardItemIdsToRemove.Contains(x.Id))
//                                                     .ToListAsync();

//                _unitOfWork.Repository<OrderData>().CreateRange(orders);

//                _unitOfWork.Repository<ShoppingCard>().DeleteRange(shoppingCardItemsToRemove);

//                //Add the data to cache
//                _cacheService.SetData("orderDetailsList", orderDetailsList, DateTimeOffset.Now.AddDays(1));

//                // Get the data from the cache
//                var cachedOrderDetailsList = _cacheService.GetData<List<OrderDetails>>("orderDetailsList");
//                //TODO CreateRangeList Method 
//                _unitOfWork.Repository<OrderDetails>().CreateRangeList(cachedOrderDetailsList);

//                _unitOfWork.Complete();

//                var pathToFile = "Templates/order_confirmation.html";

//                string htmlBody = "";
//                using (StreamReader streamReader = System.IO.File.OpenText(pathToFile))
//                {
//                    htmlBody = streamReader.ReadToEnd();
//                }

//                double totalPrice = 0;
//                shoppingCardItems.ForEach(x => totalPrice += x.ProductPrice);

//                var orderIds = orders.Select(x => x.OrderId).ToList();

//                //var totalPrice = shoppingCardItems.Select(x => x.ProductPrice).Sum();
//                var orderConfirmationDto = new OrderConirmationDto
//                {
//                    UserName = "LifeUser",
//                    OrderDate = DateTime.Now,
//                    Price = totalPrice,
//                    OrderId = string.Join(",", orderIds)
//                };

//                var myData = new[] { "LifeUser", DateTime.Now.ToString(), totalPrice.ToString(), string.Join(",", orderIds) };

//                var content = string.Format(htmlBody, myData);

//                await _emailSender.SendEmailAsync(addressDetails.Email, "OrderConfirmation", content);

//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex,"An error occurred while creating shopping card item.");
//            }
//        }

//    }
//}
