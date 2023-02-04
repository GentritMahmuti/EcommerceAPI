
using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using DataAccess.UnitOfWork.IUnitOfWork;
using Domain.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using Services.DTOs.Product;
using Services.Services.IServices;

namespace Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductService> _logger;
        private readonly ElasticClient _elasticClient;
        private readonly ICacheService _cacheService;


        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, ILogger<ProductService> logger, ElasticClient elasticClient, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _elasticClient = elasticClient;
            _cacheService = cacheService;
        }


        
        /// <summary>
        /// Sets a discount to the product with specific id if it isn't null, else throws exception!
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="discountPercentage"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task ProductDiscount(int productId, int discountPercentage)
        {
            var product = await GetProduct(productId);
            if (product == null)
            {
                throw new NullReferenceException("The product you're trying to make a discount to doesn't exist!");
            }
            if (product.ListPrice - product.Price >= 0.01)
            {
                throw new Exception("The product it is at a discount, to make another discount, remove existing discount first.");
            }
            product.Price = product.ListPrice - (product.ListPrice * discountPercentage / 100);


            var key = $"Product_{productId}";
            var expirationTime = DateTimeOffset.Now.AddMinutes(30);
            _cacheService.SetUpdatedData<Product>(key, product, expirationTime);

            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.CompleteAsync();
            await UpdateElastic(_mapper.Map<ProductDto>(product));

        }

        /// <summary>
        /// Removes the discount from a specific product if it exists and has discount, else throws exception
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task RemoveProductDiscount(int productId)
        {
            var product = await GetProduct(productId);
            if (product == null)
            {
                throw new NullReferenceException("The product you're trying to make a discount doesn't exist!");
            }
            if (product.ListPrice - product.Price < 0.0001)
            {
                throw new Exception("This product is not discounted");
            }

            product.Price = product.ListPrice;

            var key = $"Product_{productId}";
            var expirationTime = DateTimeOffset.Now.AddMinutes(30);
            _cacheService.SetUpdatedData<Product>(key, product, expirationTime);

            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.CompleteAsync();
            await UpdateElastic(_mapper.Map<ProductDto>(product));

        }

        public async Task<Product> GetProduct(int id)
        {
            var key = $"Product_{id}";
            var product = _cacheService.GetData<Product>(key);
            if (product == null)
            {
                product = await _unitOfWork.Repository<Product>().GetById(x => x.Id == id).FirstOrDefaultAsync();
                var expirationTime = DateTimeOffset.Now.AddMinutes(30);
                _cacheService.SetData(key, product, expirationTime);

            }
            return product;
        }


        /// <summary>
        /// Gets products in paginated form! 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Product>> GetProductsPaginated(int pageIndex, int pageSize)
        {
            return await _unitOfWork.Repository<Product>().GetAll().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<Product>> GetAllProducts()
        {
            var products = _unitOfWork.Repository<Product>().GetAll();
            return await products.ToListAsync();
        }


        /// <summary>
        /// Gets products of a given category id.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>List of products.</returns>
        public async Task<List<Product>> GetProductsByCategory(int categoryId, int pageIndex = 1, int pageSize = 10)
        {
            var products = _unitOfWork.Repository<Product>().GetByCondition(x => x.CategoryId == categoryId).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return await products.ToListAsync();
        }
        /// <summary>
        /// Gets products which are created in the last hour!
        /// </summary>
        /// <returns>List of products</returns>
        public async Task<List<Product>> GetProductsCreatedLast()
        {
            var products = await _unitOfWork.Repository<Product>().GetByCondition(x => x.CreatedDateTime > DateTime.Now.AddHours(-1)).ToListAsync();
            _logger.LogInformation($"{nameof(ProductService)} - Got products created in the last hour!");
            return products;

        }

        public async Task CreateProduct(ProductCreateDto productToCreate)
        {
            var product = _mapper.Map<Product>(productToCreate);

            _unitOfWork.Repository<Product>().Create(product);
            await _unitOfWork.CompleteAsync();
            var key = $"Product_{product.Id}";
            var itemInCache = _cacheService.GetData<Product>(key);
            if (itemInCache == null)
            {
                //Store the data in the cache
                var expirationTime = DateTimeOffset.Now.AddMinutes(30);
                _cacheService.SetData(key, product, expirationTime);
            }
            _logger.LogInformation($"{nameof(ProductService)} - Created product successfully!");

        }

        public async Task CreateProducts(List<ProductCreateDto> productsToCreate)
        {
            var products = _mapper.Map<List<ProductCreateDto>, List<Product>>(productsToCreate);
            _unitOfWork.Repository<Product>().CreateRange(products);

            await _unitOfWork.CompleteAsync();

            foreach (var product in products)
            {
                var key = $"Product_{product.Id}";
                var expirationTime = DateTimeOffset.Now.AddMinutes(30);
                _cacheService.SetData(key, product, expirationTime);
            }

            _logger.LogInformation($"{nameof(ProductService)} - Created products successfully!");

        }

        public async Task CreateProductsFromCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("The file given is null or empty!");
            }

            var csvRecords = new List<string>();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                {
                    csvRecords.Add(reader.ReadLine());
                }
            }

            List<Product> productsToCreate = new();
            csvRecords.RemoveAt(0); // Remove the first line 
            foreach (var line in csvRecords)
            {
                var csvLine = line.Split(",");
                Product product = new Product
                {
                    Name = csvLine[0],
                    Description = csvLine[1],
                    ListPrice = double.Parse(csvLine[2]),
                    Price = double.Parse(csvLine[3]),
                    ImageUrl = csvLine[4],
                    CategoryId = int.Parse(csvLine[5]),
                    Stock = int.Parse(csvLine[6]),
                    TotalSold = int.Parse(csvLine[7]),
                    CreatedDateTime = DateTime.Parse(csvLine[8])
                };
                productsToCreate.Add(product);
            }
            _unitOfWork.Repository<Product>().CreateRange(productsToCreate);
            await _unitOfWork.CompleteAsync();
            await AddBulkElastic(productsToCreate);
            _logger.LogInformation($"{nameof(ProductService)} - Created products from csv file successfully!");
        }

        public async Task DeleteProduct(int id)
        {
            var product = await GetProduct(id);
            if (product == null)
            {
                throw new NullReferenceException("The product you're trying to delete doesn't exist.");
            }

            var key = $"Product_{id}";
            _cacheService.RemoveData(key);


            await DeleteByIdElastic(id);

            _unitOfWork.Repository<Product>().Delete(product);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"{nameof(ProductService)} - Deleted product successfully!");
        }

        /// <summary>
        /// Updates a product if it exists, else throws exception!
        /// </summary>
        /// <param name="productToUpdate"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public async Task UpdateProduct(ProductDto productToUpdate)
        {
            var product = await GetProduct(productToUpdate.Id);
            if (product == null)
            {
                throw new NullReferenceException("The product you're trying to update doesn't exist!");
            };
            product.Name = productToUpdate.Name;
            product.Stock = productToUpdate.Stock;
            product.TotalSold = productToUpdate.TotalSold;

            var key = $"Product_{product.Id}";
            var expirationTime = DateTimeOffset.Now.AddMinutes(30);
            _cacheService.SetUpdatedData<Product>(key, product, expirationTime);

           
            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.CompleteAsync();
            await UpdateElastic(productToUpdate);
            _logger.LogInformation($"{nameof(ProductService)} - Updated product successfully!");
        }


        /// <summary>
        /// Gets recommended products based on userId.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>Paginated list of products</returns>
        public async Task<List<Product>> GetRecommendedProducts(string userId, int pageIndex, int pageSize)
        {
            var reviewedCategories = await _unitOfWork.Repository<Review>().GetByCondition(x => x.UserId == userId).Select(r => r.Product.CategoryId).ToListAsync();
            if (reviewedCategories.Count > 0)
            {
                var searchResponse = await _elasticClient.SearchAsync<Product>(s => s
                .Index("products")
                    .From((pageIndex - 1) * pageSize)
                    .Size(pageSize)
                    .Query(q => q
                        .Bool(b => b
                            .Should(sh => sh
                                .Terms(t => t
                                    .Field(f => f.CategoryId)
                                    .Terms(reviewedCategories)
                                    .Boost(1.5)
                                )
                            )
                        )

                    )
                    .Sort(sort => sort
                          .Field("_score", SortOrder.Descending)
                          .Field(f => f.TotalSold, SortOrder.Descending)
                          .Field(f => f.Price , SortOrder.Ascending)
                          .Field(f => f.Category.DisplayOrder, SortOrder.Ascending)
                    )
                );
                return searchResponse.Documents.ToList();
            }

            var sortedProducts = await _elasticClient.SearchAsync<Product>(s => s
                .Index("products")
                    .From((pageIndex - 1) * pageSize)
                    .Size(pageSize)
                    .Query(q => q
                        .MatchAll())
                    .Sort(
                        sort => sort
                            .Field(f => f.TotalSold, SortOrder.Descending)
                            .Field(f => f.Category.DisplayOrder, SortOrder.Ascending))
                    );
            return sortedProducts.Documents.ToList();
        }

        
        /// <summary>
        /// Searches products based on title, filters based on price, sorts by price and\or popularity
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>Paginated list of products!</returns>
        public async Task<List<Product>> SearchElastic(SearchInputDto input, int pageIndex, int pageSize)
        {
            var minPrice = (input.MinPrice <= 0) ? 0.1 : input.MinPrice;
            var maxPrice = (input.MaxPrice <= 0) ? 10000 : input.MaxPrice;

            var response = await _elasticClient.SearchAsync<Product>(s => s
               .Index("products")
               .From((pageIndex - 1) * pageSize)
               .Size(pageSize)
               .Query(q => q
                    .Match(m => m
                        .Field(f => f.Name)
                            .Query(input.Title)
                 ) && q
                 .Range(r => r
                    .Field(f => f.Price)
                        .GreaterThanOrEquals(minPrice)
                        .LessThanOrEquals(maxPrice))
                 ).Sort(sort =>
                 {
                     if (input.SortByPopularity != null)
                     {
                         bool sortAscending = input.SortByPopularity == "ascending";
                         sort = sortAscending ? sort.Field(f => f.TotalSold, SortOrder.Ascending) : sort.Field(f => f.TotalSold, SortOrder.Descending);
                     }
                     if (input.SortByPrice != null)
                     {
                         bool sortAscending = input.SortByPrice == "ascending";
                         sort = sortAscending ? sort.Field(f => f.Price, SortOrder.Ascending) : sort.Field(f => f.Price, SortOrder.Descending);
                     }
                     return sort;
                 }
                )
            );
            _logger.LogInformation($"{nameof(ProductService)} - Searched for products using elastic successfully!");
            return response.Documents.ToList();
        }

        public async Task<List<Product>> GetAllElastic()
        {
            var response = await _elasticClient.SearchAsync<Product>(s =>
            s.Index("products")
                .Size(1000)
               .Query(q => q
                  .MatchAll())
               );
            return response.Documents.ToList();
        }


        /// <summary>
        /// Adds many products in elastic by once!
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public async Task AddBulkElastic(List<Product> products)
        {
            var result = await _elasticClient.BulkAsync(x =>
                x.Index("products").IndexMany(products));
            _logger.LogInformation($"{nameof(ProductService)} - Added bulk of products in elastic successfully!");
        }

        /// <summary>
        /// Updates the product in elastic!
        /// </summary>
        /// <param name="productToCreate"></param>
        /// <returns></returns>
        public async Task UpdateElastic(ProductDto productToCreate)
        {
            var product = _mapper.Map<Product>(productToCreate);

            var updatedResponse = _elasticClient.Update<Product>(product.Id, x => x
                        .Index("products")
                        .Doc(product));

            if (!updatedResponse.IsValid)
            {
                _logger.LogError($"{nameof(ProductService)} - Update failed: " + updatedResponse.DebugInformation);
            }
            else
            {
                _logger.LogInformation($"{nameof(ProductService)} - Updated product in elastic successfully!");
            }

        }

        /// <summary>
        /// Updates stock and totalSold properties of a product in elastic.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="stock"></param>
        /// <param name="totalSold"></param>
        /// <returns></returns>
        public async Task UpdateSomeElastic(int productId, int stock, int totalSold)
        {
            var updateResponse = _elasticClient.Update<Product>(productId, u => u
                .Index("products")
                .Doc(new Product
                {
                    Stock = stock,
                    TotalSold = totalSold
                })
            );

            if (!updateResponse.IsValid)
            {
                _logger.LogError($"{nameof(ProductService)} - Update failed: " + updateResponse.DebugInformation);
            }
            else
            {
                _logger.LogInformation($"{nameof(ProductService)} - Updated product in elastic successfully!");
            }
        }
        public async Task DeleteAllElastic()
        {
            var deleteResponse = _elasticClient.DeleteByQuery<Product>(del => del
                   .Index("products")
                   .Query(q => q.MatchAll())
               );

            _logger.LogInformation($"{nameof(ProductService)} - Deleted all products from elastic successfully!");
        }

        public async Task DeleteByIdElastic(int id)
        {
            var deleteResponse = _elasticClient.Delete<Product>(id, d => d.Index("products"));

            if (deleteResponse.IsValid)
            {
                _logger.LogInformation($"{nameof(ProductService)} - Deleted product with Id: {id} from elastic successfully!");
            }
            else
            {
                _logger.LogError($"{nameof(ProductService)} - Error during deletion of product with Id: {id} using elastic!", deleteResponse.DebugInformation);
            }
        }

        /// <summary>
        /// Uploads image in the blob, sets the url of that image as ImageUrl of a specific product if it exists.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="productId"></param>
        /// <returns>The url of the image.</returns>
        /// <exception cref="NullReferenceException"></exception>
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
            await _unitOfWork.CompleteAsync();
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
