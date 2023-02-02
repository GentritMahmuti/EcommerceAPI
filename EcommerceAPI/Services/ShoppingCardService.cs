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
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Product = EcommerceAPI.Models.Entities.Product;
using System.Text;
using EcommerceAPI.Models.DTOs.Promotion;
using EcommerceAPI.Models.DTOs.Product;
using Microsoft.EntityFrameworkCore;
using Nest;
using EcommerceAPI.Controllers;

namespace EcommerceAPI.Services
{
    public class ShoppingCardService : IShoppingCardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ShoppingCardService> _logger;
        private readonly ICacheService _cacheService;
        private readonly IProductService _productService;
        private List<string> _keys;

        public ShoppingCardService(IUnitOfWork unitOfWork, IMapper mapper, IEmailSender emailSender, ILogger<ShoppingCardService> logger, ICacheService cacheService, IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailSender = emailSender;
            _logger = logger;
            _cacheService = cacheService;
            _keys = new List<string>();
            _productService = productService;
        }


        /// <summary>
        /// Gets a specific card item given its id.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns>A cartItem</returns>
        private async Task<CartItem> GetCardItem(int itemId)
        {

            var cartItem = await _unitOfWork.Repository<CartItem>()
                .GetById(x => x.CartItemId == itemId)
                .AsNoTracking()
                .Include("Product")
                .FirstOrDefaultAsync();

            return cartItem;
        }


        /// <summary>
        /// Adds a product to card given its id.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task AddProductToCard(string userId, int productId, int count)
        {

            var product = await _productService.GetProduct(productId);

            if (product.Stock < count)
            {
                throw new Exception("Stock is not sufficient.");
            }

            var shoppingCardItem = new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Count = count
            };

            _unitOfWork.Repository<CartItem>().Create(shoppingCardItem);
            await _unitOfWork.CompleteAsync();



            var cartItem = await GetCardItem(shoppingCardItem.CartItemId);

            //Check if the data is already in the cache
            var key = $"CartItems_{userId}";

            //Store the data in the cache
            _cacheService.SetDataMember(key, cartItem);


        }


        /// <summary>
        /// Gets all details about the products that a user has in his shoppingCard.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>ShoppingCardDetails</returns>
        public async Task<List<CartItem>> GetShoppingCardItems(string userId)
        {
            try
            {
                // Log the key
                var key = $"CartItems_{userId}";

            var key = $"CartItems_{userId}";

            // Check if the data is already in the cache
            var usersShoppingCardItems = _cacheService.GetDataSet<CartItem>(key);

            // If not, then get the data from the database
            if (usersShoppingCardItems.Count == 0)
            {
                usersShoppingCardItems = await _unitOfWork.Repository<CartItem>()
                                                                    .GetByCondition(x => x.UserId == userId)
                                                                    .AsNoTracking()
                                                                    .Include(x => x.Product)
                                                                    .ToListAsync();

                foreach (var cartItem in usersShoppingCardItems)
                {
                    _cacheService.SetDataMember(key, cartItem);
                }
            }
            return usersShoppingCardItems;
        }

        public async Task<ShoppingCardDetails> GetShoppingCardContentForUser(string userId)
        {
            try
            { 
                
                var usersShoppingCard = await GetShoppingCardItems(userId);

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
                    CardTotal = shoppingCardList.Select(x => x.Total).Sum(),
                    ItemCount = shoppingCardList.Count
                };
                return shoppingCardDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ShoppingCardService)} - There was an error while trying to get the shopping card content!");
                return new ShoppingCardDetails();
            }
        }

        /// <summary>
        /// Removes a specific product from shoppingCard.
        /// </summary>
        /// <param name="shoppingCardItemId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task RemoveProductFromCard(int shoppingCardItemId, string userId)
        {
            try
            {
                var cacheKey = $"CartItems_{userId}";
                var shoppingCardItem = await CheckRedisAndDatabaseForData(shoppingCardItemId, cacheKey);

                _unitOfWork.Repository<CartItem>().Delete(shoppingCardItem);
                await _unitOfWork.CompleteAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ShoppingCardService)} - An error occured while trying to remove a product to card");
                throw new Exception("An error occurred while removing the item from the cart");
            }
        }


        /// <summary>
        /// Empties the shoppingCard of a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task RemoveAllProductsFromCard(string userId)
        {
            var key = $"CartItems_{userId}";

            var usersShoppingCard = _cacheService.GetDataSet<CartItem>(key);
            _cacheService.RemoveData(key);
            _unitOfWork.Repository<CartItem>().DeleteRange(usersShoppingCard);
            await _unitOfWork.CompleteAsync();

        }

        /// <summary>
        /// Increases the quantity of a product in shoppingCard of the user with userId.
        /// </summary>
        /// <param name="shoppingCardItemId"></param>
        /// <param name="userId"></param>
        /// <param name="newQuantity"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task IncreaseProductQuantityInShoppingCard(int shoppingCardItemId, string userId, int? newQuantity)
        {
            var cacheKey = $"CartItems_{userId}";
            var shoppingCardItem = await CheckRedisAndDatabaseForData(shoppingCardItemId, cacheKey);

            if (newQuantity == null)
                shoppingCardItem.Count++;
            else
                shoppingCardItem.Count = (int)newQuantity;

            _unitOfWork.Repository<CartItem>().Update(shoppingCardItem);
            await _unitOfWork.CompleteAsync();
            var cartItem = await GetCardItem(shoppingCardItem.CartItemId);
            _cacheService.SetDataMember(cacheKey, cartItem);


        }
        public async Task DecreaseProductQuantityInShoppingCard(int shoppingCardItemId, string userId, int? newQuantity)
        {

            var cacheKey = $"CartItems_{userId}";
            var shoppingCardItem = await CheckRedisAndDatabaseForData(shoppingCardItemId, cacheKey);

            if (shoppingCardItem == null)
            {
                throw new Exception("Cart item not found in the database.");
            }
            if (newQuantity == null)
                shoppingCardItem.Count--;
            else
                shoppingCardItem.Count = (int)newQuantity;

            _unitOfWork.Repository<CartItem>().Update(shoppingCardItem);
            await _unitOfWork.CompleteAsync();
            var cartItem = await GetCardItem(shoppingCardItem.CartItemId);
            _cacheService.SetDataMember(cacheKey, cartItem);


        }
        private async Task<CartItem> CheckRedisAndDatabaseForData(int shoppingCardItemId, string cachekey)
        {
            CartItem? shoppingCardItem = null;
            var usersShoppingCard = _cacheService.GetDataSet<CartItem>(cachekey);
            if (usersShoppingCard != null)
            {
                foreach (var item in usersShoppingCard)
                {
                    if (item.CartItemId == shoppingCardItemId)
                    {
                        shoppingCardItem = item;
                        _cacheService.RemoveDataFromSet(cachekey, item);
                    }
                }
            }

            if (shoppingCardItem == null)
            {
                shoppingCardItem = await _unitOfWork.Repository<CartItem>()
                                                    .GetById(x => x.CartItemId == shoppingCardItemId)
                                                    .FirstOrDefaultAsync();
            }
            return shoppingCardItem;

        }

    }
}
