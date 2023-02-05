using Domain.Entities;
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

        /// <summary>
        /// Returns the content of the wishlist for a user.
        /// </summary>
        /// <returns>A list of products that have been saved by the user</returns>
        /// <response code="200">The content of the wishlist was returned successfully</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the wishlist was not found for the user</response>
        /// <response code="400">If there was an error when getting the wishlist items for this user</response>
        /// <tags>Wishlist </tags>
        /// <remarks>
        /// This action requires authentication.
        /// </remarks>

        [HttpGet("GetWishlistContent")]
        public async Task<ActionResult<List<Product>>> GetWishlistContent()
        {
            try
            {
                var userData = (ClaimsIdentity)User.Identity;
                var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;
                var products = await _wishlistService.GetWishlistContent(userId);
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


        /// <summary>
        /// Adds a product to the wishlist of the user.
        /// </summary>
        /// <param name="productId">The ID of the product to be added to the wishlist.</param>
        /// <returns>A message indicating if the product was successfully added to the wishlist.</returns>
        /// <response code="200">The product was successfully added to the wishlist.</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="400">If there was an error adding the product to the wishlist.</response>
        /// <tags>Wishlist </tags>
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


        /// <summary>
        /// Removes a product from the wishlist of the current user
        /// </summary>
        /// <param name="productId">Id of the product to be removed</param>
        /// <returns>A message indicating that the product was removed from the wishlist</returns>
        /// <response code="200">The product was removed from the wishlist successfully</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="400">If there was an error while removing the product from wishlist</response>
        /// <tags>Wishlist</tags>
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


        /// <summary>
        /// Converts the wishlist products to shoppingCard items
        /// </summary>
        /// <param name="productId">Id of the product to be converted</param>
        /// <returns>A message indicating that the product was converted from the wishlist</returns>
        /// <response code="200">The product was removed from the wishlist successfully</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="400">If there was an error while converting the product from wishlist</response>
        /// <tags>Wishlist</tags>
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


