using EcommerceAPI.Services;
using EcommerceAPI.Services.IServices;

namespace EcommerceAPI.Helpers
{
    public static class StartupHelper
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IShoppingCardService, ShoppingCardService>();
            services.AddTransient<IOrderDataService, OrderDataService>();
            services.AddTransient<IOrderDetailsService, OrderDetailsService>();
        }
       
    }
}
