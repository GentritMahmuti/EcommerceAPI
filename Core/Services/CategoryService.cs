
using AutoMapper;
using Persistence.UnitOfWork.IUnitOfWork;
using Domain.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.DTOs.Category;
using Services.Services.IServices;

namespace Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }


        /// <summary>
        /// Creates a category from the given dto.
        /// </summary>
        /// <param name="categoryToCreate"></param>
        /// <returns></returns>
        public async Task CreateCategory(CategoryCreateDto categoryToCreate)
        {
            var category = _mapper.Map<Category>(categoryToCreate);

            _unitOfWork.Repository<Category>().Create(category);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"{nameof(CategoryService)} - Created category successfully!");
        }


        /// <summary>
        /// Gets a specific category by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A category</returns>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<Category> GetCategory(int id)
        {
            var category = await _unitOfWork.Repository<Category>().GetById(x => x.CategoryId == id).FirstOrDefaultAsync();

            if (category == null)
            {
                throw new NullReferenceException("There is no category with the given id.");
            }

            return category;
        }


        /// <summary>
        /// Gets all existing categories.
        /// </summary>
        /// <returns>List of categories</returns>
        public async Task<List<Category>> GetAllCategories()
        {
            var categories = _unitOfWork.Repository<Category>().GetAll();
            return categories.ToList();
        }


        /// <summary>
        /// Updates a specific category by id if it exists, else throws exception.
        /// </summary>
        /// <param name="categoryToUpdate"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public async Task UpdateCategory(CategoryDto categoryToUpdate)
        {
           
            var category = await GetCategory(categoryToUpdate.CategoryId);
            if (category == null)
            {
                throw new NullReferenceException("The category you're trying to update doesn't exist!");
            }

            category.CategoryName = categoryToUpdate.CategoryName;
            category.DisplayOrder= categoryToUpdate.DisplayOrder;

            _unitOfWork.Repository<Category>().Update(category);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"{nameof(CategoryService)} - Updated category successfully!");

        }


        /// <summary>
        /// Deletes a specific catogory by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task DeleteCategory(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid Category Id");
            }

            var category = await GetCategory(id);
            if (category == null)
            {
                throw new NullReferenceException("The category you're trying to delete doesn't exist.");
            }

            _unitOfWork.Repository<Category>().Delete(category);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"{nameof(CategoryService)} - Deleted category successfully!");
        }
    }
}
