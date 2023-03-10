

using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Services.DTOs.Product;

namespace Services.Services.IServices
{
    public interface IProductService
    {
        Task ProductDiscount(int productId, int discountPercentage);
        Task RemoveProductDiscount(int productId);
        Task CreateProduct(ProductCreateDto productToCreate);
        Task CreateProducts(List<ProductCreateDto> productsToCreate);
        Task CreateProductsFromCsv(IFormFile file);
        Task DeleteProduct(int id);
        Task<List<Product>> GetProductsCreatedLast();
        Task<List<Product>> GetAllProducts();
        Task<Product> GetProduct(int id);
        Task<List<Product>> GetProductsPaginated(int pageIndex, int pageSize);
        Task<List<Product>> GetProductsByCategory(int categoryId, int pageIndex, int pageSize);
        Task<List<Product>> GetRecommendedProducts(string userId, int pageIndex, int pageSize);
        Task UpdateProduct(ProductDto productToUpdate);
        Task<string> UploadImage(IFormFile? file, int productId);
        

        //Elastic
        Task<List<Product>> SearchElastic(SearchInputDto input, int pageIndex, int pageSize);
        Task<List<Product>> GetAllElastic();
        Task AddBulkElastic(List<Product> productsToCreate);
        Task UpdateElastic(ProductDto productToCreate);
        Task UpdateSomeElastic(int productId, int stock, int totalSold);

        Task DeleteAllElastic();
        
    }
}
