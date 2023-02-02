using EcommerceAPI.Models.Entities;
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

        [HttpGet("GetSavedItemContent")]
        public async Task<ActionResult<List<Product>>> GetSavedItemsContent()
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userIdClaim = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userIdClaim == null) { return Unauthorized(); }

            try
            {
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

        [HttpPost("AddToSavedItems")]
        public async Task<IActionResult> AddProductToSavedItems(int productId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userIdClaim = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userIdClaim == null) { return Unauthorized(); }

            try
            {
                await _savedItemService.AddProductToSavedItems(userIdClaim, productId);
                return Ok("Added to saved items!");
            }
            catch (Exception ex)
            {
                _logger.LogError("There was an error while adding the product to saved items");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("RemoveProductFromSavedItems")]
        public async Task<IActionResult> RemoveProductFromSavedItems(int productId)
        {
            var userData = (ClaimsIdentity)User.Identity;
            var userIdClaim = userData.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userIdClaim == null) { return Unauthorized(); }

            try
            {
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