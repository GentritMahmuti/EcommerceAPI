﻿using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.IServices;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishListController : Controller
    {
        private readonly IWishlistService _wishlistService;
        private readonly ILogger<WishListController> _logger;

        public WishListController(IWishlistService wishlistService, ILogger<WishListController> logger)
        {
            _wishlistService = wishlistService;
            _logger = logger;
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
                    _logger.LogInformation("Wishlist was not found!");
                    return NotFound("Wishlist not found for user");
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error while retrieving the products from your wishlist");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddToWishlist")]
        public async Task<IActionResult> AddProductToWishlist(int productId)
        {
            try
            {
                var userData = (ClaimsIdentity)User.Identity;
                var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _wishlistService.AddProductToWishlist(userId, productId);
                return Ok("Added to wishlist!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error while adding the product to your wishlist");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("RemoveFromWishlist")]
        public async Task<IActionResult> RemoveProductFromWishlist(int productId)
        {
            try
            {
                var userData = (ClaimsIdentity)User.Identity;
                var userId= userData.FindFirst(ClaimTypes.NameIdentifier).Value;

                await _wishlistService.RemoveProductFromWishlist(userId, productId);
                return Ok("Removed from wishlist!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error while removing the product from your wishlist");
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


