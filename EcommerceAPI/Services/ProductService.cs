using AutoMapper;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EcommerceAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductService> _logger;


        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<List<Product>> GetFilterProducts(ProductFilter filter, ProductSort sort)
        {
            var query = _unitOfWork.Repository<Product>().GetAll().AsQueryable();

            // Apply the filters
            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            }
            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            }
            if (filter.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
            }

            // Apply the sorting
            if (!string.IsNullOrEmpty(sort.SortBy))
            {
                string sortExpression = sort.SortBy;
                if (!sort.Ascending)
                {
                    sortExpression += " descending";
                }
                query = query.OrderBy(sortExpression);
            }

            // Execute the query and return the results
            return await query.ToListAsync();
        }


        public async Task<Product> GetProduct(int id)
        {
            Expression<Func<Product, bool>> expression = x => x.Id == id;
            var product = await _unitOfWork.Repository<Product>().GetById(expression).FirstOrDefaultAsync();

            return product;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            var products = _unitOfWork.Repository<Product>().GetAll();
            return products.ToList();
        }


        public async Task CreateProduct(ProductCreateDto productToCreate)
        {
            var product = _mapper.Map<Product>(productToCreate);

            _unitOfWork.Repository<Product>().Create(product);
            _unitOfWork.Complete();
            _logger.LogInformation("Created product successfully!");

        }


        public async Task CreateProducts(List<ProductCreateDto> productsToCreate)
        {
            var products = _mapper.Map<List<ProductCreateDto>, List<Product>>(productsToCreate);
            _unitOfWork.Repository<Product>().CreateRange(products);
            _unitOfWork.Complete();
            _logger.LogInformation("Created products successfully!");

        }

        public async Task DeleteProduct(int id)
        {
            var product = await GetProduct(id);
            if (product == null)
            {
                throw new NullReferenceException("The product you're trying to delete doesn't exist.");
            }

            _unitOfWork.Repository<Product>().Delete(product);
            _unitOfWork.Complete();
            _logger.LogInformation("Deleted product successfully!");

        }

        public async Task UpdateProduct(Product productToUpdate)
        {
            var product = await GetProduct(productToUpdate.Id);
            if (product == null)
            {
                throw new NullReferenceException("The product you're trying to update doesn't exist!");
            }
            product.Title = productToUpdate.Title;

            _unitOfWork.Repository<Product>().Update(product);

            _unitOfWork.Complete();
        }
    }
    
}
