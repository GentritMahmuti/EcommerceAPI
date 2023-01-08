using AutoMapper;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Helpers;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

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
        public async Task<PagedInfo<Product>> ProductsListView(string search, int page, int pageSize, int categoryId = 0)
        {
            Expression<Func<Product, bool>> condition = x => x.Title.Contains(search);

            IQueryable<Product> products;


            if (categoryId != 0)
            {
                Expression<Func<Product, bool>> conditionByCategory = x => x.CategoryId == categoryId;
                products = _unitOfWork.Repository<Product>()
                                             .GetByCondition(conditionByCategory).WhereIf(!string.IsNullOrEmpty(search), condition);
            }
            else // dismiss category
            {
                products = _unitOfWork.Repository<Product>().GetAll().WhereIf(!string.IsNullOrEmpty(search), condition);
            }

            var count = await products.CountAsync();

            var categoriesPaged = new PagedInfo<Product>()
            {
                TotalCount = count,
                Page = page,
                PageSize = pageSize,
                Data = await products
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize).ToListAsync()
            };

            return categoriesPaged;
        }

    }

}
