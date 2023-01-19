using EcommerceAPI.Models.Entities;
using FluentValidation;

namespace EcommerceAPI.Validators
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} must not be null!")
                .NotEmpty().WithMessage("{PropertyName} must not be empty!")
                .GreaterThan(0).WithMessage("{PropertyName} must be positive");

            RuleFor(p => p.Title)
                .NotNull().WithMessage("{PropertyName} must not be null!")
                .NotEmpty().WithMessage("{PropertyName} must not be empty!")
                .MaximumLength(250).WithMessage("{PropertyName} can have maximum 250 characters");
            
            RuleFor(p => p.ListPrice)
               .NotNull().WithMessage("{PropertyName} must not be null!")
               .NotEmpty().WithMessage("{PropertyName} must not be empty!")
               .ExclusiveBetween(0, 10000).WithMessage("Product {PropertyName} must be between 0 and 10000!");

            RuleFor(p => p.Price)
               .NotNull().WithMessage("{PropertyName} must not be null!")
               .NotEmpty().WithMessage("{PropertyName} must not be empty!")
               .ExclusiveBetween(0, 10000).WithMessage("Product {PropertyName} must be between 0 and 10000!");

            RuleFor(p => p.CategoryId)
               .NotNull().WithMessage("{PropertyName} must not be null!")
               .NotEmpty().WithMessage("{PropertyName} must not be empty!")
               .GreaterThan(0).WithMessage("{PropertyName} must be positive");

            RuleFor(p => p.CoverTypeId)
               .NotNull().WithMessage("{PropertyName} must not be null!")
               .NotEmpty().WithMessage("{PropertyName} must not be empty!")
               .GreaterThan(0).WithMessage("{PropertyName} must be positive");
        }
    }
}