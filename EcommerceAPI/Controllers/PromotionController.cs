using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Promotion;
using Services.Services.IServices;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : Controller
    {
        private readonly IPromotionService _promotionService;
        private readonly IValidator<PromotionDto> _promotionValidator;
        private readonly ILogger<PromotionController> _logger;

        public PromotionController(IPromotionService promotionService, IValidator<PromotionDto> promotionValidator, ILogger<PromotionController> logger)
        {
            _promotionService = promotionService;
            _promotionValidator = promotionValidator;
            _logger = logger;
        }



        /// <summary>
        /// Gets the details of a specific promotion
        /// </summary>
        /// <param name="id">The id of the promotion to retrieve</param>
        /// <returns>The details of the specified promotion</returns>
        /// <response code="200">The promotion was retrieved successfully</response>
        /// <response code="400">An error occurred while retrieving the promotion</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Promotion</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" or "LifeUser" role to access.
        /// </remarks>
        [Authorize]
        [HttpGet("GetPromotion")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var promotion = await _promotionService.GetPromotionDetails(id);

                return Ok(promotion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PromotionController)} - Error when getting a promotion!");
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Gets a list of all promotions
        /// </summary>
        /// <returns>A list of promotions</returns>
        /// <response code="200">The list of promotions was retrieved successfully</response>
        /// <response code="400">An error occurred while retrieving the promotions</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Promotion</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" role to access.
        /// </remarks>
        [Authorize(Roles = "LifeAdmin")]
        [HttpGet("GetPromotions")]
        public async Task<IActionResult> GetPromotions()
        {
            try
            {
                var promotions = await _promotionService.GetAllPromotions();

                return Ok(promotions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PromotionController)} - Error when getting all promotions!");
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Creates a new promotion
        /// </summary>
        /// <param name="createPromotion">The information for the new promotion</param>
        /// <returns>A message indicating if the promotion was created successfully</returns>
        /// <response code="200">The promotion was created successfully</response>
        /// <response code="400">An error occurred while creating the promotion</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Promotion</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" role to access.
        /// </remarks>
        [Authorize(Roles = "LifeAdmin")]
        [HttpPost("CreatePromotion")]
        public async Task<IActionResult> CreatePromotion(PromotionDto createPromotion)
        {
            try
            {
                await _promotionValidator.ValidateAndThrowAsync(createPromotion);
                await _promotionService.CreatePromotion(createPromotion);

                return Ok("Promotion created successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates a promotion
        /// </summary>
        /// <param name="id">The id of the promotion to be updated</param>
        /// <param name="updatePromotion">The updated information for the promotion</param>
        /// <returns>A message indicating if the promotion was updated successfully</returns>
        /// <response code="200">The promotion was updated successfully</response>
        /// <response code="400">An error occurred while updating the promotion</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Promotion</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" role to access.
        /// </remarks>
        [Authorize(Roles = "LifeAdmin")]
        [HttpPut("UpdatePromotion/{id}")]
        public async Task<IActionResult> Update(int id, PromotionDto updatePromotion)
        {
            try
            {
                await _promotionValidator.ValidateAndThrowAsync(updatePromotion);
                await _promotionService.UpdatePromotion(id, updatePromotion);
                return Ok("Promotion updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PromotionController)} - Error when updating a promotion!");
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Deletes a promotion
        /// </summary>
        /// <param name="id">The id of the promotion to delete</param>
        /// <returns>A message indicating if the promotion was deleted successfully</returns>
        /// <response code="200">The promotion was deleted successfully</response>
        /// <response code="400">An error occurred while deleting the promotion</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Promotion</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" role to access.
        /// </remarks>
        [Authorize(Roles = "LifeAdmin")]
        [HttpDelete("DeletePromotion")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _promotionService.DeletePromotion(id);
                return Ok("Promotion deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PromotionController)} - Error when deleting a promotion!");
                return BadRequest("Error deleting promotion: " + ex.Message);
            }
        }
    }
}
