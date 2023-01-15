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
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IConfiguration _configuration;
        private readonly IValidator<Category> _categoryValidator;

        public CategoryController(ICategoryService categoryService, IConfiguration configuration, IValidator<Category> categoryValidator)
        {
            _categoryService = categoryService;
            _configuration = configuration;
            _categoryValidator = categoryValidator;
        }

        [HttpGet("GetCategory")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _categoryService.GetCategory(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        //[HttpGet("GetCategoryWithIncludes")]
        //public async Task<IActionResult> GetIncludes(int id)
        //{
        //    var category = await _categoryService.GetWithIncludes(id);

        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(category);
        //}
        [Authorize(Roles = "LifeAdmin")]
        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategories();

            return Ok(categories);
        }

        //[HttpGet("CategorysListView")]
        //public async Task<IActionResult> CategorysListView(string? search, int categoryId = 0, int page = 1, int pageSize = 10)
        //{
        //    var categorys = await _categoryService.CategorysListView(search, page, pageSize, categoryId);

        //    return Ok(categorys);
        //}

        [HttpPost("PostCategory")]
        public async Task<IActionResult> Post(CategoryDto createCategory)
        {
            await _categoryService.CreateCategory(createCategory);

            return Ok("Category created successfully!");
        }

        [HttpPut("UpdateCategory/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDto categoryDto)
        {
            try
            {
                var category = await _categoryService.GetCategory(id);
                if (category == null)
                {
                    return NotFound($"Category with id {id} not found");
                }
                category.Name = categoryDto.Name;
                category.DisplayOrder = categoryDto.DisplayOrder;
                await _categoryValidator.ValidateAndThrowAsync(category);
                await _categoryService.UpdateCategory(category);
                return Ok("Category updated successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"The update failed: '{ex.Message}'");
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
