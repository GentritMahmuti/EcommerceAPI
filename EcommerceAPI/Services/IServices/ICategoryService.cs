using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface ICategoryService
    {
        Task CreateCategory(CategoryCreateDto categoryToCreate);
        Task DeleteCategory(int id);
        Task<List<Category>> GetAllCategories();
        Task<Category> GetCategory(int id);
        Task UpdateCategory(Category categoryToUpdate);
    }
}
