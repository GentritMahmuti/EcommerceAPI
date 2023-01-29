using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.DTOs.Promotion;
using EcommerceAPI.Models.DTOs.Review;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Validators.DtoValidators.Category;
using EcommerceAPI.Validators.DtoValidators.Order;
using EcommerceAPI.Validators.DtoValidators.Product;
using EcommerceAPI.Validators.DtoValidators.Promotion;
using EcommerceAPI.Validators.DtoValidators.Review;
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
           
            services.AddScoped<IValidator<PromotionDto>, PromotionValidator>();

            // DtoValidators
            services.AddScoped<IValidator<CategoryCreateDto>, CategoryCreateDtoValidator>();
            services.AddScoped<IValidator<CategoryDto>, CategoryDtoValidator>();
            
            services.AddScoped<IValidator<AddressDetails>, AddressDetailsValidator>();

            services.AddScoped<IValidator<ProductCreateDto>, ProductCreateDtoValidator>();
            services.AddScoped<IValidator<ProductDto>, ProductDtoValidator>();

            services.AddScoped<IValidator<PromotionDetailsDto>, PromotionDetailsDtoValidator>();
            services.AddScoped<IValidator<PromotionDto>, PromotionDtoValidator>();

            services.AddScoped<IValidator<ReviewCreateDto>, ReviewCreateDtoValidator>();
            services.AddScoped<IValidator<ReviewDetailsDto>, ReviewDetailsDtoValidator>();
            services.AddScoped<IValidator<ReviewUpdateDto>, ReviewUpdateDtoValidator>();

            
        }
    }
}
