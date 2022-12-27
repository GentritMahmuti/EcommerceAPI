using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface IProductService
    {
        Task CreateProduct(ProductCreateDto productToCreate);
        Task DeleteProduct(int id);
        Task<List<Product>> GetAllProducts();
        //Task<PagedInfo<Product>> ProductsListView(string search, int page, int pageSize, int categoryId);
        Task<Product> GetProduct(int id);
        Task UpdateProduct(Product productToUpdate);
        //Task<Product> GetWithIncludes(int id);
    }
}
