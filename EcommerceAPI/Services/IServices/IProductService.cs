using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.Entities;
using Nest;

namespace EcommerceAPI.Services.IServices
{
    public interface IProductService
    {
        Task ProductDiscount(int productId, int discountPercentage);
        Task RemoveProductDiscount(int productId);

        Task<List<Product>> GetFilterProducts(ProductFilter filter, ProductSort sort);
        Task CreateProduct(ProductCreateDto productToCreate);
        Task DeleteProduct(int id);
        Task<List<Product>> GetProductsCreatedLast();
        Task<List<Product>> GetAllProducts();
        //Task<PagedInfo<Product>> ProductsListView(string search, int page, int pageSize, int categoryId);
        Task<Product> GetProduct(int id);
        Task UpdateProduct(Product productToUpdate);
        //Task<Product> GetWithIncludes(int id);
        Task<string> UploadImage(IFormFile? file, int productId);
        


        //Elastic
        Task<List<Product>> SearchElastic(SearchInputDto input, int pageIndex, int pageSize);
        Task<List<Product>> GetAllElastic();
        Task AddBulkElastic(List<Product> productsToCreate);
        Task UpdateElastic(ProductDto productToCreate);
        Task DeleteAllElastic();
        
    }
}
