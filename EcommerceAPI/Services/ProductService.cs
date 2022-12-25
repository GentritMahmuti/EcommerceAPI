using AutoMapper;

namespace EcommerceAPI.Services
{
    public class ProductService
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
            Expression<Func<Product, bool>> expression = x => x.ProductId == id;
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
        
    }
    
}
