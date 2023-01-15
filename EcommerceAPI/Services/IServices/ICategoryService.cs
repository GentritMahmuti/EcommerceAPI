using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface ICategoryService
    {
        Task CreateCategory(CategoryDto productToCreate);
        Task DeleteCategory(int id);
        Task<List<Category>> GetAllCategories();
        //Task<PagedInfo<Category>> CategoriesListView(string search, int page, int pageSize, int categoryId);
        Task<Category> GetCategory(int id);
        Task UpdateCategory(Category productToUpdate);
        //Task<Category> GetWithIncludes(int id);
    }
}
