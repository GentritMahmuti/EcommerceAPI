using AutoMapper;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Helpers;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Nest;
using EcommerceAPI.Helpers;
using Microsoft.IdentityModel.Tokens;
using Amazon.S3;
using Amazon.S3.Model;
using System.ComponentModel.DataAnnotations;
using static Nest.JoinField;

namespace EcommerceAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductService> _logger;
        private readonly ElasticClient _elasticClient;


        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, ILogger<ProductService> logger, ElasticClient elasticClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _elasticClient = elasticClient;
        }

        public async Task ProductDiscount(int productId, int discountPercentage)
        {
            var product = await GetProduct(productId);
            if (product == null)
            {
                throw new NullReferenceException("The product you're trying to make a discount doesn't exist!");
            }
            if(product.ListPrice - product.Price >= 0.01)
            {
                throw new Exception("The product it is at a discount, to make another discount, remove existing discount first.");
            }
            product.Price = product.ListPrice - (product.ListPrice * discountPercentage / 100);
           
            _unitOfWork.Repository<Product>().Update(product);

            _unitOfWork.Complete();
        }
        public async Task RemoveProductDiscount(int productId)
        {
            var product = await GetProduct(productId);
            if (product == null)
            {
                throw new NullReferenceException("The product you're trying to make a discount doesn't exist!");
            }
            if(product.ListPrice - product.Price < 0.0001)
            {
                throw new Exception("This product is not discounted");
            }

            product.Price = product.ListPrice;

            _unitOfWork.Repository<Product>().Update(product);

            _unitOfWork.Complete();
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

        public async Task<PagedInfo<Product>> ProductsListView(string search, int page, int pageSize, int categoryId = 0)
        {
            Expression<Func<Product, bool>> condition = x => x.Title.Contains(search);

            IQueryable<Product> products;


            if (categoryId is not 0)
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


        // Elastic
        public async Task<List<Product>> SearchElastic(SearchInputDto input, int pageIndex, int pageSize)
        {
            
            var response = await _elasticClient.SearchAsync<Product>(s => s
               .Index("products")
               .From((pageIndex - 1) * pageSize)
               .Size(pageSize)
               .Query(q => q
                    .Match(m => m
                        .Field(f => f.Title)
                            .Query(input.Title)
                 ) && q
                 .Range(r => r
                    .Field(f => f.Price)
                        .GreaterThanOrEquals(input.MinPrice)
                        .LessThanOrEquals(input.MaxPrice))
                 )
            );
            _logger.LogInformation("Searched for products using elastic successfully!");
            return response.Documents.ToList();
        }

        public async Task<IndexResponse> AddProductElastic(ProductDto productToCreate)
        {
            var product = _mapper.Map<Product>(productToCreate);
            var result = await _elasticClient.IndexAsync
              (product, x => x.Index("products").Id(product.Id));

            if (result.IsValid)
            {
                _logger.LogInformation("Added product using elastic successfully!");
            }
            return result;
        }

        public async Task<Product> GetByIdElastic(int id, string index)
        {
            var response = await _elasticClient.GetAsync<Product>(id, x => x.Index(index));
            if (response.Source != null)
            {
                _logger.LogInformation("Found product by id using elastic successfully!");
            }
            return response.Source;
        }

        public async Task<List<Product>> GetAllElastic()
        {
            var response = await _elasticClient.SearchAsync<Product>(s =>
            s.Index("products")
               .Query(q => q
                  .MatchAll())
               );
            return response.Documents.ToList();
        }

        public async Task AddBulkElastic(List<ProductDto> products)
        {
            var productsToCreate = _mapper.Map<List<ProductDto>, List<Product>>(products);
            var result = await _elasticClient.BulkAsync(x =>
                x.Index("products").IndexMany(productsToCreate));
            _logger.LogInformation("Added bulk of products in elastic successfully!");
        }

        public async Task UpdateElastic(ProductDto productToCreate)
        {
            var product = _mapper.Map<Product>(productToCreate);

            var response = await _elasticClient.GetAsync<Product>(product.Id, x => x.Index("products"));
            var existingProduct = response.Source;

            existingProduct.Title = product.Title;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Category = product.Category;

            var updatedResponse = _elasticClient.Update<Product>(product.Id, x => x
                        .Index("products")
                        .Doc(existingProduct));
            _logger.LogInformation("Updated product from elastic successfully!");

        }

        public async Task DeleteAllElastic()
        {
            var deleteResponse = _elasticClient.DeleteByQuery<Product>(del => del
                   .Index("products")
                   .Query(q => q.MatchAll())
               );
            _logger.LogInformation("Deleted all products from elastic successfully!");
        }
        public async Task DeleteProductByIdInElastic(int id)
        {
            var deleteResponse = _elasticClient.Delete<Product>(id, d => d
            .Index("products")
            );
            if (!deleteResponse.IsValid)
            {
                throw new Exception("There isn't a product with that ID");
            }
        }

        public async Task<string> UploadImage(IFormFile? file, int productId)
        {
            var uploadPicture = await UploadToBlob(file, file.FileName, Path.GetExtension(file.FileName));
            var imageUrl = $"{_configuration.GetValue<string>("BlobConfig:CDNLife")}{file.FileName}";

            var product = await GetProduct(productId);
            if (product is null)
            {
                throw new NullReferenceException("There is no product with the given id!");
            }
            product.ImageUrl = imageUrl;

            _unitOfWork.Repository<Product>().Update(product);
            _unitOfWork.Complete();
            _logger.LogInformation($"{nameof(ProductService)}: Uploaded Image of the product successfully!");

            return imageUrl;
        }


        public async Task<PutObjectResponse> UploadToBlob(IFormFile? file, string name, string extension)
        {
            string serviceURL = _configuration.GetValue<string>("BlobConfig:serviceURL");
            string AWS_accessKey = _configuration.GetValue<string>("BlobConfig:accessKey");
            string AWS_secretKey = _configuration.GetValue<string>("BlobConfig:secretKey");
            var bucketName = _configuration.GetValue<string>("BlobConfig:bucketName");
            var keyName = _configuration.GetValue<string>("BlobConfig:defaultFolder");

            var config = new AmazonS3Config() { ServiceURL = serviceURL };
            var s3Client = new AmazonS3Client(AWS_accessKey, AWS_secretKey, config);
            keyName = String.Concat(keyName, name);

            var fs = file.OpenReadStream();
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = keyName,
                InputStream = fs,
                ContentType = $"image/{extension}",
                CannedACL = S3CannedACL.PublicRead
            };
            _logger.LogInformation("File is uploaded to blob successfully!");
            return await s3Client.PutObjectAsync(request);
        }

        
    }
}
