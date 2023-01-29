using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.Entities;
using Nest;

namespace EcommerceAPI.Services.IServices
{
    public interface IProductService
    {
        Task CreateOrderForProduct(string userId, int productId, int count, AddressDetails addressDetails);
        Task ProductDiscount(int productId, int discountPercentage);
        Task RemoveProductDiscount(int productId);

        Task CreateProduct(ProductCreateDto productToCreate);
        Task DeleteProduct(int id);
        Task<List<Product>> GetProductsCreatedLast();
        Task<List<Product>> GetAllProducts();
        Task<Product> GetProduct(int id);
        Task UpdateProduct(ProductDto productToUpdate);
        Task<string> UploadImage(IFormFile? file, int productId);
        

        //Elastic
        Task<List<Product>> SearchElastic(SearchInputDto input, int pageIndex, int pageSize);
        Task<List<Product>> GetAllElastic();
        Task AddBulkElastic(List<Product> productsToCreate);
        Task UpdateElastic(ProductDto productToCreate);
        Task DeleteAllElastic();
        
    }
}
