using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services;
using EcommerceAPI.Services.IServices;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishListController : Controller
    {
        private readonly IWishlistService _wishlistService;

        public WishListController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet("GetWishlistContent")]
        public async Task<ActionResult<List<Product>>> GetWishlistContent()
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userIdClaim = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userIdClaim == null) { return Unauthorized(); }

            try
            {
                var products = await _wishlistService.GetWishlistContent(userIdClaim);
                if (products == null)
                {
                    return NotFound("Wishlist not found for user");
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddToWishlist")]
        public async Task<IActionResult> AddProductToWishlist(int productId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userIdClaim = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userIdClaim == null) { return Unauthorized(); }

            try
            {
                await _wishlistService.AddProductToWishlist(userIdClaim, productId);
                return Ok("Added to wishlist!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("RemoveFromWishlist")]
        public async Task<IActionResult> RemoveProductFromWishlist(int productId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userIdClaim = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userIdClaim == null) { return Unauthorized(); }

            try
            {
                await _wishlistService.RemoveProductFromWishlist(userIdClaim, productId);
                return Ok("Removed from wishlist!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddToShoppingCard")]
        public async Task<IActionResult> AddToCardFromWishlist(int productId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var product = await _wishlistService.GetProductFromWishlist(productId);
                if (product == null)
                {
                    return NotFound();
                }
                await _wishlistService.AddToCardFromWishlist(userId, productId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }





    }
}


