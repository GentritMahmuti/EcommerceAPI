using EcommerceAPI.Models.DTOs.Promotion;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    public class PromotionController : Controller
    {
        private readonly IPromotionService _promotionService;
        private readonly IConfiguration _configuration;

        public PromotionController(IPromotionService promotionService, IConfiguration configuration)
        {
            _promotionService = promotionService;
            _configuration = configuration;
        }

        [HttpGet("GetPromotion")]
        public async Task<IActionResult> Get(int id)
        {
            var promotion = await _promotionService.GetPromotion(id);

            if (promotion == null)
            {
                return NotFound();
            }

            return Ok(promotion);
        }

        //[Authorize(Roles = "LifeAdmin")]
        [HttpGet("GetPromotions")]
        public async Task<IActionResult> GetPromotions()
        {
            var promotions = await _promotionService.GetAllPromotions();

            return Ok(promotions);
        }

        [HttpPost("PostPromotion")]
        public async Task<IActionResult> Post(PromotionCreateDto createPromotion)
        {
            await _promotionService.CreatePromotion(createPromotion);

            return Ok("Promotion created successfully!");
        }

        [HttpPut("UpdatePromotion/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Promotion promotionDto)
        {
            try
            {
                var promotion = await _promotionService.GetPromotion(id);
                if (promotion == null)
                {
                    return NotFound($"Promotion with id {id} not found");
                }
                promotion.Name = promotionDto.Name;
                promotion.StartDate = promotionDto.StartDate;
                promotion.EndDate = promotionDto.EndDate;
                promotion.DiscountAmount = promotionDto.DiscountAmount;
                
     
                await _promotionService.UpdatePromotion(promotion);
                return Ok("Promotion updated successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"The update failed: '{ex.Message}'");
            }
        }

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
