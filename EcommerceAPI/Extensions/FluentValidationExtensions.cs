using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.Promotion;
using EcommerceAPI.Models.DTOs.Review;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Validators.DtoValidators.Category;
using EcommerceAPI.Validators.DtoValidators.Order;
using EcommerceAPI.Validators.EntityValidators;
using FluentValidation;

namespace EcommerceAPI.Extensions
{
    public static class FluentValidationExtensions
    {
        public static void AddFluentValidations(this IServiceCollection services)
        {
            // EntityValidators
            services.AddScoped<IValidator<Category>, CategoryValidator>();
            services.AddScoped<IValidator<EcommerceAPI.Models.Entities.Product>, ProductValidator>();
            //services.AddScoped<IValidator<OrderDetails>, OrderDetailsValidator>();
            services.AddScoped<IValidator<ReviewCreateDto>, ReviewCreateDtoValidator>();
            services.AddScoped<IValidator<ReviewUpdateDto>, ReviewUpdateDtoValidator>();
            services.AddScoped<IValidator<PromotionDto>, PromotionValidator>();

            // DtoValidators
            services.AddScoped<IValidator<CategoryCreateDto>, CategoryCreateDtoValidator>();
            services.AddScoped<IValidator<CategoryDto>, CategoryDtoValidator>();
            
            services.AddScoped<IValidator<AddressDetails>, AddressDetailsValidator>();
        }
    }
}
