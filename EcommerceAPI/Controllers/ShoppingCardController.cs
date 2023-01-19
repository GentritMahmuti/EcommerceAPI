using EcommerceAPI.Models.DTOs.ShoppingCard;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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


        [HttpGet("ShoppingCardContent")]
        public async Task<IActionResult> ShoppingCardContent()
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCardDetails? shoppingCardContentForUser = await _cardService.GetShoppingCardContentForUser(userId);

            return Ok(shoppingCardContentForUser);
        }

        [HttpPost("IncreaseQuantityForProduct")]
        public async Task<IActionResult> Plus(int? newQuantity, int shoppingCardItemId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            await _cardService.Plus(shoppingCardItemId, newQuantity);

            return Ok();
        }

        [HttpPost("DecreaseQuantityForProduct")]
        public async Task<IActionResult> Minus(int? newQuantity, int shoppingCardItemId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId == null) { return Unauthorized(); }

            await _cardService.Minus(shoppingCardItemId, newQuantity);

            return Ok();
        }

        [HttpPost("ProductSummaryForOrder")]
        public async Task<IActionResult> ProductSummary(ProductSummaryModel model)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            model.AddressDetails.Email = userData.FindFirst(ClaimTypes.Email).Value;

            if (userId == null) { return Unauthorized(); }

            await _cardService.CreateOrder(model.AddressDetails, model.ShoppingCardItems);

            return Ok();
        }


    }
}
