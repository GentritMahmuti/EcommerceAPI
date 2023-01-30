using EcommerceAPI.Models.DTOs.ShoppingCard;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public ShoppingCardController(IShoppingCardService cardService, IConfiguration configuration)
        {
            _cardService = cardService;
            _configuration = configuration;
        }

        [HttpPost("AddToCard")]
        public async Task<IActionResult> AddProductToCard(int count, int productId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            await _cardService.AddProductToCard(userId, productId, count);

            return Ok("Added to card!");
        }

        [HttpDelete("RemoveFromCard")]
        public async Task<IActionResult> RemoveProductFromCard(int shoppingCardItemId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }
            
            await _cardService.RemoveProductFromCard(shoppingCardItemId, userId);

            return Ok("Removed from card!");
        }

        [HttpDelete("RemoveAllProductsFromCard")]
        public async Task<IActionResult> RemoveAllProductsFromCard()
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            await _cardService.RemoveAllProductsFromCard(userId);

            return Ok("All products removed from card!");
        }


        [HttpGet("ShoppingCardContent")]
        public async Task<IActionResult> ShoppingCardContent()
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCardDetails? shoppingCardContentForUser = await _cardService.GetShoppingCardContentForUser(userId);

            return Ok(shoppingCardContentForUser);
        }

        [HttpPost("IncreaseQuantityForProduct")]
        public async Task<IActionResult> IncreaseProductQuantity(int? newQuantity, int shoppingCardItemId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            await _cardService.IncreaseProductQuantityInShoppingCard(shoppingCardItemId, userId, newQuantity);

            return Ok();
        }

        [HttpPost("DecreaseQuantityForProduct")]
        public async Task<IActionResult> DecreaseProductQuantity(int? newQuantity, int shoppingCardItemId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            await _cardService.DecreaseProductQuantityInShoppingCard(shoppingCardItemId, userId, newQuantity);

            return Ok();
        }

        [HttpPost("ProductSummaryForOrder")]
        public async Task<IActionResult> ProductSummary(ProductSummaryModel model)
        {
            try
            {
                var userData = (ClaimsIdentity)User.Identity;
                var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

                model.AddressDetails.Email = userData.FindFirst(ClaimTypes.Email).Value;

                if (userId == null) { return Unauthorized(); }

                await _cardService.CreateOrder(userId, model.AddressDetails, model.PromoCode);

                return Ok();
            } 
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
