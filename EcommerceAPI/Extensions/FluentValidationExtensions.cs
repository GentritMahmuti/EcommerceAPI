
using Domain.Entities;
using EcommerceAPI.Validators.DtoValidators.Category;
using EcommerceAPI.Validators.DtoValidators.Order;
using EcommerceAPI.Validators.DtoValidators.Product;
using EcommerceAPI.Validators.DtoValidators.Promotion;
using EcommerceAPI.Validators.DtoValidators.Review;
using EcommerceAPI.Validators.EntityValidators;
using FluentValidation;
using Services.DTOs.Category;
using Services.DTOs.Order;
using Services.DTOs.Product;
using Services.DTOs.Promotion;
using Services.DTOs.Review;

namespace EcommerceAPI.Extensions
{
    public static class FluentValidationExtensions
    {
        public static void AddFluentValidations(this IServiceCollection services)
        {
            // EntityValidators
            services.AddScoped<IValidator<Category>, CategoryValidator>();
            services.AddScoped<IValidator<Domain.Entities.Product>, ProductValidator>();
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
