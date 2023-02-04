using EcommerceAPI.Services;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.ShoppingCard;
using Services.Services.IServices;
using Stripe;
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
        /// Adds a product to the user's shopping card.
        /// </summary>
        /// <param name="count">The quantity of the product to be added to the shopping card.</param>
        /// <param name="productId">The id of the product to be added to the shopping card.</param>
        /// <returns>Ok result if the product was added to the shopping card, 
        /// a BadRequest result with a message if an error occurred, or an Unauthorized result if the user is not authenticated.</returns>
        [Authorize]
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
        /// Removes a product from the shopping card
        /// </summary>
        /// <param name="shoppingCardItemId">The id of the shopping card item to remove</param>
        /// <returns>A message indicating if the item was removed successfully</returns>
        /// <response code="200">The item was removed successfully</response>
        /// <response code="400">An error occurred while removing the item</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the shopping card item could not be found</response>
        /// <tags>Shopping Card</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" or "LifeUser" role to access.
        /// </remarks>
        [Authorize]
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
        /// Remove all products from shopping card
        /// </summary>
        /// <returns>A message indicating if all products were removed successfully</returns>
        /// <response code="200">The products were removed successfully</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Shopping Card</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" or "LifeUser" role to access.
        /// </remarks>
        [Authorize]
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
        /// Gets the shopping card content for the current user
        /// </summary>
        /// <returns>The shopping card content for the current user</returns>
        /// <response code="200">The shopping card content was retrieved successfully</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <response code="404">If the shopping card content is not found</response>
        /// <tags>Shopping Card</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" or "LifeUser" role to access.
        /// </remarks>
        [Authorize]
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
        /// Increases the quantity of a product in the shopping card
        /// </summary>
        /// <param name="newQuantity">The new quantity for the product</param>
        /// <param name="shoppingCardItemId">The id of the shopping card item</param>
        /// <returns>An indication of whether the quantity was increased successfully</returns>
        /// <response code="200">The quantity was increased successfully</response>
        /// <response code="400">An error occurred while increasing the quantity</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Shopping Card</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" or "LifeUser" role to access.
        /// </remarks>
        [Authorize]
        [HttpPost("IncreaseQuantityForProduct")]
        public async Task<IActionResult> IncreaseProductQuantity(int? newQuantity, int shoppingCardItemId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            try
            {
                await _cardService.IncreaseProductQuantityInShoppingCard(shoppingCardItemId, userId, newQuantity);

                return Ok("The product quantity in the shopping card changed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ShoppingCardController)} - An error occured while trying to add a product to card");
                return BadRequest("An error happened: " + ex.Message);

            }
        }

        /// <summary>
        /// This method is responsible for decreasing the quantity of a product in the shopping cart.
        /// </summary>
        /// <param name="newQuantity">The new quantity to be set for the product in the cart</param>
        /// <param name="shoppingCardItemId">The unique identifier of the item in the shopping cart</param>
        /// <returns>Ok result if the operation was successful, Unauthorized result if the user is not authorized,
        /// BadRequest if an error occurs while decreasing the quantity of the product in the cart</returns>
        [Authorize]
        [HttpPost("DecreaseQuantityForProduct")]
        public async Task<IActionResult> DecreaseProductQuantity(int? newQuantity, int shoppingCardItemId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            try
            {
                await _cardService.DecreaseProductQuantityInShoppingCard(shoppingCardItemId, userId, newQuantity);
                return Ok("The product quantity in the shopping card changed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ShoppingCardController)} - An error occured while trying to remove one product to card");
                return BadRequest("An error happened: " + ex.Message);
            }
        }

        [Authorize]
        [HttpPost("ConvertToWishList")]
        public async Task<IActionResult> ConvertToWishList(int cartItemId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            try
            {
                var wishListItem = await _cardService.ConvertToWishList(cartItemId);
                return Ok(wishListItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ShoppingCardController)} - An error occured while trying to convert a product to wishlist");
                return BadRequest("An error happened: " + ex.Message);
            }
        }
    }
}
