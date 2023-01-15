using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.Entities;
using Nest;

namespace EcommerceAPI.Services.IServices
{
    public interface IProductService
    {
        Task<List<Product>> GetFilterProducts(ProductFilter filter, ProductSort sort);
        Task CreateProduct(ProductCreateDto productToCreate);
        Task DeleteProduct(int id);
        Task<List<Product>> GetAllProducts();
        //Task<PagedInfo<Product>> ProductsListView(string search, int page, int pageSize, int categoryId);
        Task<Product> GetProduct(int id);
        Task UpdateProduct(Product productToUpdate);
        //Task<Product> GetWithIncludes(int id);
        Task<string> UploadImage(IFormFile? file, int productId);


        //Elastic
        Task<List<Product>> SearchElastic(SearchInputDto input, int pageSize);
        Task<IndexResponse> AddProductElastic(ProductCreateElasticDto product);
        Task<Product> GetByIdElastic(int id, string index);
        Task<List<Product>> GetAllElastic();
        Task AddBulkElastic(List<ProductCreateElasticDto> productsToCreate);
        Task UpdateElastic(ProductCreateElasticDto productToCreate);
        Task DeleteAllElastic();
    }
}
