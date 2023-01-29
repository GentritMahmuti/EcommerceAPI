using EcommerceAPI.Models.DTOs.Product;
using FluentValidation;

namespace EcommerceAPI.Validators.DtoValidators.Product
{
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        public ProductDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(c => c.Name)
                .NotNull().WithMessage("{PropertyName} must not be null!")
                .NotEmpty().WithMessage("{PropertyName} must not be empty!")
                .Length(1, 150).WithMessage("{PropertyName} must be between 1 and 150 characters!");

            RuleFor(c => c.Description)
                .NotNull().WithMessage("{PropertyName} must not be null!")
                .NotEmpty().WithMessage("{PropertyName} must not be empty!")
                .Length(1, 10000).WithMessage("{PropertyName} must be between 1 and 10000 characters!");

            RuleFor(c => c.ListPrice)
                .NotNull().WithMessage("{PropertyName} must not be null!")
                .NotEmpty().WithMessage("{PropertyName} must not be empty!")
                .GreaterThan(0).WithMessage("{PropertyName} must be positive!");

            RuleFor(c => c.Price)
                .NotNull().WithMessage("{PropertyName} must not be null!")
                .NotEmpty().WithMessage("{PropertyName} must not be empty!")
                .GreaterThan(0).WithMessage("{PropertyName} must be positive!");

            RuleFor(c => c.CategoryId)
               .NotNull().WithMessage("{PropertyName} must not be null!")
               .NotEmpty().WithMessage("{PropertyName} must not be empty!")
               .GreaterThan(0).WithMessage("{PropertyName} must be positive!");
        }
    }
}
