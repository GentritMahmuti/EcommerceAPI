using EcommerceAPI.Services.IServices;
using EcommerceAPI.Services;
using Stripe;

namespace EcommerceAPI.Infrastructure
{
    public static class StripeInfrastructure
    {
        public static IServiceCollection AddStripeInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            StripeConfiguration.ApiKey = configuration.GetValue<string>("StripeSettings:SecretKey");

            return services
                .AddScoped<CustomerService>()
                .AddScoped<ChargeService>()
                .AddScoped<TokenService>()
                .AddScoped<IStripeAppService, StripeAppService>();
        }
    }
}
