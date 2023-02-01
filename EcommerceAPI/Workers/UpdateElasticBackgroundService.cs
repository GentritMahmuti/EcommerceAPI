using EcommerceAPI.Services.IServices;
using Nest;

namespace EcommerceAPI.Workers
{
    public class UpdateElasticBackgroundService : IHostedService, IDisposable
    {

        private IServiceProvider _serviceProvider;
        private Timer? _timer = null;
        private readonly ILogger _logger;

        public UpdateElasticBackgroundService(IServiceProvider serviceProvider, ElasticClient elasticClient, ILogger<UpdateElasticBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(AddProductsCreatedLast, null, TimeSpan.Zero,
              TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);    
            return Task.CompletedTask;
        }

        private async void AddProductsCreatedLast(object? state)
        {
           using var scope = _serviceProvider.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

            try
            {
                var products = await productService.GetProductsCreatedLast();
                await productService.AddBulkElastic(products);
                _logger.LogInformation("Added products created in the last hour to elastic!");
            }catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UpdateElasticBackgroundService)} : An error happened when adding the product created in the last hour!");
            }
           
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

    }
}
