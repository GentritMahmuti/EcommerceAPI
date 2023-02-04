using EcommerceAPI.Services.IServices;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Category;
using Services.Services.IServices;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IConfiguration _configuration;
        private readonly IValidator<CategoryDto> _categoryValidator;
        private readonly IValidator<CategoryCreateDto> _categoryCreateValidator;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, IConfiguration _configuration, IValidator<CategoryDto> categoryValidator, IValidator<CategoryCreateDto> categoryCreateValidator, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _configuration = _configuration;
            _categoryValidator = categoryValidator;
            _categoryCreateValidator = categoryCreateValidator;
            _logger = logger;
        }


        /// <summary>
        /// Creates a category!
        /// </summary>
        /// <param name="createCategory"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeAdmin")]
        [HttpPost("PostCategory")]
        public async Task<IActionResult> Post(CategoryCreateDto createCategory)
        {
            try
            {
                await _categoryCreateValidator.ValidateAndThrowAsync(createCategory);
                await _categoryService.CreateCategory(createCategory);
                return Ok("Category created successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CategoryController)} - Error when creating a category!");
                return BadRequest("An error happened: " + ex.Message);
            }
        }


        /// <summary>
        /// Gets a category with a specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A category.</returns>
        [HttpGet("GetCategory")]
        [Authorize(Roles = "LifeAdmin, LifeUser")]
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
                _logger.LogError(ex, $"{nameof(CategoryController)} - Error when creating a category!");
                return BadRequest("An error happened: " + ex.Message);
            }
        }


        /// <summary>
        /// Gets all categories from db!
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "LifeAdmin, LifeUser")]
        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategories();

            return Ok(categories);
        }


        /// <summary>
        /// Updates a specific category by id. 
        /// </summary>
        /// <param name="categoryToUpdate"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeAdmin")]
        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> Update(CategoryDto categoryToUpdate)
        {
            try
            {
                await _categoryValidator.ValidateAndThrowAsync(categoryToUpdate);
                await _categoryService.UpdateCategory(categoryToUpdate);
                return Ok("Category updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CategoryController)} - Error when updating a category!");
                return BadRequest("An error happened: " + ex.Message);
            }
        }


        /// <summary>
        /// Deletes a specific category by id!
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "LifeAdmin")]
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
                _logger.LogError(ex, $"{nameof(CategoryController)} - Error when deleting a category!");
                return BadRequest("Error deleting category: " + ex.Message);
            }
        }
    }
}
