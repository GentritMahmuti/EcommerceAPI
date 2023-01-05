using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface IProductService
    {
        Task<List<Product>> GetFilterProducts(ProductFilter filter, ProductSort sort);
        Task CreateProduct(ProductCreateDto productToCreate);
        Task DeleteProduct(int id);
        Task<List<Product>> GetAllProducts();
        Task<Product> GetProduct(int id);
        Task UpdateProduct(Product productToUpdate);
    }
}
