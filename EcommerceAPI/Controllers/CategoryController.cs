using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using EcommerceAPI.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IValidator<Category> _categoryValidator;

        public CategoryController(ICategoryService categoryService, IValidator<Category> categoryValidator)
        {
            _categoryService = categoryService;
            _categoryValidator = categoryValidator;
        }

        [HttpPost("PostCategory")]
        public async Task<IActionResult> Post(CategoryCreateDto createCategory)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _categoryService.CreateCategory(createCategory);
                return Ok("Category created successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCategory")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var category = await _categoryService.GetCategory(id);

                if (category == null)
                {
                    return NotFound();
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategories();

            return Ok(categories);
        }

        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> Update(CategoryDto categoryToUpdate)
        {
            try
            {
                await _categoryService.UpdateCategory(categoryToUpdate);
                return Ok("Category updated successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpDelete("DeleteCategory")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _categoryService.DeleteCategory(id);
                return Ok("Category deleted successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest("Error deleting category: " + ex.Message);
            }
        }
    }
}
