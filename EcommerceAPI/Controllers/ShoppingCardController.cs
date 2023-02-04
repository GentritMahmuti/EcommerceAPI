using EcommerceAPI.Services;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.ShoppingCard;
using Services.Services.IServices;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShoppingCardController : ControllerBase
    {
        private readonly IShoppingCardService _cardService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ShoppingCardController> _logger;

        public ShoppingCardController(IShoppingCardService cardService, IConfiguration configuration, ILogger<ShoppingCardController> logger)
        {
            _cardService = cardService;
            _configuration = configuration;
            _logger = logger;
        }


        /// <summary>
        /// Adds a product to card.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        [Authorize("Roles = LifeAdmin, LifeUser")]
        [HttpPost("AddToCard")]
        public async Task<IActionResult> AddProductToCard(int count, int productId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            try 
            { 
                await _cardService.AddProductToCard(userId, productId, count);

                return Ok("Added to card!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ShoppingCardController)} - An error occured while trying to add a product to card");
                return BadRequest("An error happened: " + ex.Message);
            }
        }

        /// <summary>
        /// Removes a product from card if it exists, else returns BadRequest();
        /// </summary>
        /// <param name="shoppingCardItemId"></param>
        /// <returns></returns>
        [Authorize("Roles = LifeAdmin, LifeUser")]
        [HttpDelete("RemoveFromCard")]
        public async Task<IActionResult> RemoveProductFromCard(int shoppingCardItemId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }
            
            try
            {
                await _cardService.RemoveProductFromCard(shoppingCardItemId, userId);
                return Ok("Removed from card!");
            }catch(Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ShoppingCardController)} - An error occured while trying to remove a product to card");
                return BadRequest("An error happened: " + ex.Message);
            }

        }

        /// <summary>
        /// Empties the shoppingCard of the user.
        /// </summary>
        /// <returns></returns>
        [Authorize("Roles = LifeAdmin, LifeUser")]
        [HttpDelete("RemoveAllProductsFromCard")]
        public async Task<IActionResult> RemoveAllProductsFromCard()
        {
            try
            {
                var userData = (ClaimsIdentity)User.Identity;
                var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (userId == null) { return Unauthorized(); }

                await _cardService.RemoveAllProductsFromCard(userId);

                return Ok("All products removed from card!");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Gets shoppingCard details that a user has.
        /// </summary>
        /// <returns></returns>
        [Authorize("Roles = LifeAdmin, LifeUser")]
        [HttpGet("ShoppingCardContent")]
        public async Task<IActionResult> ShoppingCardContent()
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCardDetails? shoppingCardContentForUser = await _cardService.GetShoppingCardContentForUser(userId);

            if (shoppingCardContentForUser == null)
            {
                return NotFound();
            }

            return Ok(shoppingCardContentForUser);
        }


        /// <summary>
        /// Increases the quantity of a product in shoppingCard of a user.
        /// </summary>
        /// <param name="newQuantity"></param>
        /// <param name="shoppingCardItemId"></param>
        /// <returns></returns>
        [Authorize("Roles = LifeAdmin, LifeUser")]
        [HttpPost("IncreaseQuantityForProduct")]
        public async Task<IActionResult> IncreaseProductQuantity(int? newQuantity, int shoppingCardItemId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            try
            {
                await _cardService.IncreaseProductQuantityInShoppingCard(shoppingCardItemId, userId, newQuantity);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ShoppingCardController)} - An error occured while trying to add a product to card");
                return BadRequest("An error happened: " + ex.Message);

            }
        }

        /// <summary>
        /// Decreases the quantity of a product in shopppingCard of a user.
        /// </summary>
        /// <param name="newQuantity"></param>
        /// <param name="shoppingCardItemId"></param>
        /// <returns></returns>
        [Authorize("Roles = LifeAdmin, LifeUser")]
        [HttpPost("DecreaseQuantityForProduct")]
        public async Task<IActionResult> DecreaseProductQuantity(int? newQuantity, int shoppingCardItemId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            try
            {
                await _cardService.DecreaseProductQuantityInShoppingCard(shoppingCardItemId, userId, newQuantity);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ShoppingCardController)} - An error occured while trying to remove one product to card");
                return BadRequest("An error happened: " + ex.Message);
            }
        }
    }
}
