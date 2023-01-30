using AutoMapper;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace EcommerceAPI.Services
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

        public async Task CreateCategory(CategoryCreateDto categoryToCreate)
        {
            if (categoryToCreate == null)
            {
                throw new ArgumentNullException(nameof(categoryToCreate));
            }

            if (string.IsNullOrWhiteSpace(categoryToCreate.CategoryName))
            {
                throw new ArgumentException("Category name must not be empty");
            }

            if(categoryToCreate.DisplayOrder <= 0)
            {
                throw new ArgumentException("DisplayOrder must be a positive number");
            }

            var category = _mapper.Map<Category>(categoryToCreate);

            _unitOfWork.Repository<Category>().Create(category);
            _unitOfWork.Complete();
            _logger.LogInformation("Created category successfully!");
        }

        public async Task<Category> GetCategory(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Id must be a positive number");
            }

            Expression<Func<Category, bool>> expression = x => x.CategoryId == id;
            var category = await _unitOfWork.Repository<Category>().GetById(expression).FirstOrDefaultAsync();

            return category;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            var categorys = _unitOfWork.Repository<Category>().GetAll();
            return categorys.ToList();
        }

        public async Task UpdateCategory(CategoryDto categoryToUpdate)
        {
            if (categoryToUpdate == null)
            {
                _logger.LogError("Input category is null");
                throw new ArgumentNullException("Input category is null");
            }

            if (categoryToUpdate.CategoryId <= 0)
            {
                _logger.LogError("Invalid Category Id");
                throw new ArgumentException("Invalid Category Id");
            }

            var category = await GetCategory(categoryToUpdate.CategoryId);
            if (category == null)
            {
                throw new NullReferenceException("The category you're trying to update doesn't exist!");
            }

            if (string.IsNullOrWhiteSpace(categoryToUpdate.CategoryName))
            {
                throw new ArgumentException("Invalid Category Name");
            }

            category.CategoryName = categoryToUpdate.CategoryName;

            _unitOfWork.Repository<Category>().Update(category);

            _unitOfWork.Complete();
        }

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
            _unitOfWork.Complete();
            _logger.LogInformation("Deleted category successfully!");
        }
    }
}
