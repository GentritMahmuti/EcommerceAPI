using AutoMapper;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcommerceAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CategoryService> _logger;


        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<Category> GetCategory(int id)
        {
            Expression<Func<Category, bool>> expression = x => x.CategoryId == id;
            var category = await _unitOfWork.Repository<Category>().GetById(expression).FirstOrDefaultAsync();

            return category;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            var categorys = _unitOfWork.Repository<Category>().GetAll();
            return categorys.ToList();
        }

        //test
        public async Task CreateCategory(CategoryCreateDto categoryToCreate)
        {
            var category = _mapper.Map<Category>(categoryToCreate);

            _unitOfWork.Repository<Category>().Create(category);
            _unitOfWork.Complete();
            _logger.LogInformation("Created category successfully!");

        }

        public async Task DeleteCategory(int id)
        {
            var category = await GetCategory(id);
            if (category == null)
            {
                throw new NullReferenceException("The category you're trying to delete doesn't exist.");
            }

            _unitOfWork.Repository<Category>().Delete(category);
            _unitOfWork.Complete();
            _logger.LogInformation("Deleted category successfully!");

        }

        public async Task UpdateCategory(Category categoryToUpdate)
        {
            var category = await GetCategory(categoryToUpdate.CategoryId);
            if (category == null)
            {
                throw new NullReferenceException("The category you're trying to update doesn't exist!");
            }
            category.CategoryName = categoryToUpdate.CategoryName;

            _unitOfWork.Repository<Category>().Update(category);

            _unitOfWork.Complete();
        }
    }
}
