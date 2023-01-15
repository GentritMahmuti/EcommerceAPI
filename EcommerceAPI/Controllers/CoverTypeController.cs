using EcommerceAPI.Models.DTOs.CoverType;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoverTypeController : ControllerBase
    {
        private readonly ICoverTypeService _coverTypeService;
        private readonly IConfiguration _configuration;
        private readonly IValidator<CoverType> _coverTypeValidator;

        public CoverTypeController(ICoverTypeService coverTypeService, IConfiguration configuration, IValidator<CoverType> coverTypeValidator)
        {
            _coverTypeService = coverTypeService;
            _configuration = configuration;
            _coverTypeValidator = coverTypeValidator;
        }

        [HttpGet("GetCoverType")]
        public async Task<IActionResult> Get(int id)
        {
            var coverType = await _coverTypeService.GetCover(id);

            if (coverType == null)
            {
                return NotFound();
            }

            return Ok(coverType);
        }

        [HttpGet("GetCoverTypes")]
        public async Task<IActionResult> GetCoverTypes()
        {
            var coverTypes = await _coverTypeService.GetAllCovers();

            return Ok(coverTypes);
        }

        [HttpPost("CreateCoverType")]
        public async Task<IActionResult> CreateCoverType(CoverTypeCreateDTO coverToCreate)
        {
            await _coverTypeService.CreateCover(coverToCreate);

            return Ok("Cover Type Created");
        }

        [HttpPut("UpdateCoverType")]
        public async Task<IActionResult> UpdateCoverType(CoverTypeDTO coverToUpdate)
        {
            await _coverTypeService.UpdateCover(coverToUpdate);

            return Ok("Cover Type Updated");
        }

        [HttpDelete("DeleteCoverType")]
        public async Task<IActionResult> DeleteCoverType(int id)
        {
            await _coverTypeService.DeleteCover(id);

            return Ok("Cover Type Deleted");
        }
    }
    
}
