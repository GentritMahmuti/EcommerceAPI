using EcommerceAPI.Models.DTOs.Promotion;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : Controller
    {
        private readonly IPromotionService _promotionService;
        private readonly IConfiguration _configuration;
        private readonly IValidator<PromotionDto> _promotionValidator;

        public PromotionController(IPromotionService promotionService, IConfiguration configuration, IValidator<PromotionDto> promotionValidator)
        {
            _promotionService = promotionService;
            _configuration = configuration;
            _promotionValidator = promotionValidator;
        }

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
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
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
                return BadRequest(ex.Message);
            }
        }
        
        [Authorize(Roles = "LifeAdmin")]
        [HttpPost("PostPromotion")]
        public async Task<IActionResult> Post(PromotionDto createPromotion)
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
        
        [Authorize(Roles = "LifeAdmin")]
        [HttpPut("UpdatePromotion/{id}")]
        public async Task<IActionResult> Update(int id, PromotionDto updatePromotion)
        {
            try
            {
                await _promotionValidator.ValidateAndThrowAsync(updatePromotion);
                await _promotionService.UpdatePromotion(id, updatePromotion);
                return Ok("Promotion updated successfully!");
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
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
                return BadRequest("Error deleting promotion: " + ex.Message);
            }
        }
    }
}
