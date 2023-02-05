using Domain.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SavedItemController : Controller
    {
        private readonly ISavedItemService _savedItemService;
        private readonly ILogger<SavedItemController> _logger;

        public SavedItemController(ISavedItemService savedItemService, ILogger<SavedItemController> logger)
        {
            _savedItemService = savedItemService;
            _logger = logger;
        }


        /// <summary>
        /// Returns the content of the saved items for a user.
        /// </summary>
        /// <returns>A list of products that have been saved by the user</returns>
        /// <response code="200">The content of the saved items was returned successfully</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the saved item list was not found for the user</response>
        /// <response code="400">If there was an error when getting the saved items for this user</response>
        /// <tags>Saved Item</tags>
        /// <remarks>
        /// This action requires authentication.
        /// </remarks>
        [Authorize]
        [HttpGet("GetSavedItemContent")]
        public async Task<ActionResult<List<Product>>> GetSavedItemsContent()
        {
            try
            {
                var userData = (ClaimsIdentity)User.Identity;
                var userIdClaim = userData.FindFirst(ClaimTypes.NameIdentifier).Value;
                var products = await _savedItemService.GetSavedItemsContent(userIdClaim);
                if (products == null)
                {
                    _logger.LogInformation("The saved item list was not found");
                    return NotFound("Saved item list not found for user");
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError("There was an error when getting the saved items for this user!");
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Adds a product to the saved items of a user.
        /// </summary>
        /// <param name="productId">The ID of the product to be added to the saved items list</param>
        /// <returns>A message indicating if the product was successfully added to the saved items list</returns>
        /// <response code="200">The product was successfully added to the saved items list</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="400">If there was an error adding the product to the saved items list</response>
        /// <tags>Saved Item</tags>
        [HttpPost("AddToSavedItems")]
        public async Task<IActionResult> AddProductToSavedItems(int productId)
        {
            try
            {
                var userData = (ClaimsIdentity)User.Identity;
                var userId = userData.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _savedItemService.AddProductToSavedItems(userId, productId);
                return Ok("Added to saved items!");
            }
            catch (Exception ex)
            {
                _logger.LogError("There was an error while adding the product to saved items");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Removes a product from the saved items of the current user
        /// </summary>
        /// <param name="productId">Id of the product to be removed</param>
        /// <returns>A message indicating that the product was removed from the saved items</returns>
        /// <response code="200">The product was removed from the saved items successfully</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="400">If there was an error while removing the product from saved items</response>
        /// <tags>Saved Items</tags>
        [HttpDelete("RemoveProductFromSavedItems")]
        public async Task<IActionResult> RemoveProductFromSavedItems(int productId)
        {
            try
            {
                var userData = (ClaimsIdentity)User.Identity;
                var userIdClaim = userData.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _savedItemService.RemoveProductFromSavedItems(userIdClaim, productId);
                return Ok("Removed from saved items!");
            }
            catch (Exception ex)
            {
                _logger.LogError("There was an error while removing the product from saved items");
                return BadRequest(ex.Message);
            }
        }
    }
}