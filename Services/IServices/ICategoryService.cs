

using Domain.Entities;
using Services.DTOs.Category;

namespace Services.Services.IServices
{
    public interface ICategoryService
    {
        Task CreateCategory(CategoryCreateDto categoryToCreate);
        Task<Category> GetCategory(int id);
        Task<List<Category>> GetAllCategories();
        Task UpdateCategory(CategoryDto categoryToUpdate);
        Task DeleteCategory(int id);
    }
}
