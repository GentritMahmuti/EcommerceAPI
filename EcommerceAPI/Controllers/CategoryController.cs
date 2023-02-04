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
        /// Creates a new category
        /// </summary>
        /// <param name="createCategory">The information for the new category</param>
        /// <returns>A message indicating if the category was created successfully</returns>
        /// <response code="200">The category was created successfully</response>
        /// <response code="400">An error occurred while creating the category</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Category</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" role to access.
        /// </remarks>
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
        /// <param name="id">The id of the category to retrieve</param>
        /// <returns>A category object</returns>
        /// <response code="200">Returns the category with the specified id</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <response code="404">If the category with the specified id is not found</response>
        /// <tags>Category</tags>
        /// <remarks>
        /// This action requires authentication and either the "LifeAdmin" or "LifeUser" role to access.
        /// </remarks>
        [Authorize]
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
                _logger.LogError(ex, $"{nameof(CategoryController)} - Error when creating a category!");
                return BadRequest("An error happened: " + ex.Message);
            }
        }


        /// <summary> 
        /// Gets all categories. 
        /// </summary>
        /// <returns> A list of categories. </returns>
        /// <response code="200">Returns the list of all categories.</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Category</tags>
        /// <remarks>
        /// This action requires authentication and either the "LifeAdmin" or "LifeUser" role to access.
        /// </remarks>
        [Authorize]
        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategories();

            return Ok(categories);
        }


        /// <summary>
        /// Updates a specific category by id.
        /// </summary>
        /// <param name="categoryToUpdate">The updated category object</param>
        /// <returns>A message indicating the status of the operation</returns>
        /// <response code="200">If the category was updated successfully</response>
        /// <response code="400">If there was an error updating the category</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Category</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" role to access.
        /// </remarks>
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
        /// Deletes a specific category by id.
        /// </summary>
        /// <param name="id">The id of the category to delete</param>
        /// <returns>A message indicating that the category has been deleted successfully</returns>
        /// <response code="200">Returns a message indicating that the category has been deleted successfully</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission to access the resources</response>
        /// <tags>Category</tags>
        /// <remarks>
        /// This action requires authentication and the "LifeAdmin" role to access.
        /// </remarks>
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
