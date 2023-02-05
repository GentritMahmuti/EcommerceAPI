using Services.Services.IServices;
using Services.Services;
using EcommerceAPI.Services.IServices;

namespace EcommerceAPI.Extensions
{
    public static class ServicesExtension
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ISavedItemService, SavedItemService>();

            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<ShoppingCardService>();

            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IShoppingCardService, ShoppingCardService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IReviewService, ReviewService>();
            services.AddTransient<IPromotionService, PromotionService>();
        }
    }
}
