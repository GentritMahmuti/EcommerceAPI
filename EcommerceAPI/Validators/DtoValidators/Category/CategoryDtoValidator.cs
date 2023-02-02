using EcommerceAPI.Models.DTOs.Category;
using FluentValidation;

namespace EcommerceAPI.Validators.DtoValidators.Category
{
    public class CategoryDtoValidator : AbstractValidator<CategoryDto>
    {
        public CategoryDtoValidator() {
              RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(c => c.CategoryId)
                .NotNull().WithMessage("{PropertyName} must not be null!")
                .NotEmpty().WithMessage("{PropertyName} must not be empty!")
                .GreaterThan(0).WithMessage("{PropertyName} must be positive");

            RuleFor(c => c.CategoryName)
                .NotNull().WithMessage("{PropertyName} must not be null!")
                .NotEmpty().WithMessage("{PropertyName} must not be empty!")
                .Length(1, 100).WithMessage("{PropertyName} must be between 1 and 100 characters!")
                .Matches(@"^[A-Za-z\s]*$").WithMessage("{PropertyName} contains invalid characters!");

            RuleFor(c => c.DisplayOrder)
                .NotNull().WithMessage("{PropertyName} must not be null!")
                .NotEmpty().WithMessage("{PropertyName} must not be empty!")
                .GreaterThan(0).WithMessage("{PropertyName} must not be empty!");
        }
    }
}
