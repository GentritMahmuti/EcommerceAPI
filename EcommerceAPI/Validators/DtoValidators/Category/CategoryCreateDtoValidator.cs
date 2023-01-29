using FluentValidation;
using EcommerceAPI.Models.DTOs.Category;

namespace EcommerceAPI.Validators.DtoValidators.Category
{
    public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
    {
        public CategoryCreateDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(c => c.CategoryName)
                    .NotNull().WithMessage("{PropertyName} must not be null!")
                    .NotEmpty().WithMessage("{PropertyName} must not be empty!")
                    .Length(1, 150).WithMessage("{PropertyName} must be between 1 and 150 characters!");          
        }
    }
}
