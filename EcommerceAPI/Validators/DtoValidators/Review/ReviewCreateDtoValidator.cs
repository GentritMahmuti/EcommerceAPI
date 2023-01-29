using EcommerceAPI.Models.DTOs.Review;
using FluentValidation;

namespace EcommerceAPI.Validators.DtoValidators.Review
{
    public class ReviewCreateDtoValidator : AbstractValidator<ReviewCreateDto>
    {
        public ReviewCreateDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(c => c.ProductId)
                .NotNull().WithMessage("{PropertyName} must not be null!")
                .NotEmpty().WithMessage("{PropertyName} must not be empty!")
                .GreaterThan(0).WithMessage("{PropertyName} must be positive!");

            RuleFor(c => c.Rating)
               .NotNull().WithMessage("{PropertyName} must not be null!")
               .NotEmpty().WithMessage("{PropertyName} must not be empty!")
               .ExclusiveBetween(1, 5).WithMessage("{PropertyName} must be between 1 and 10!");


           

        }
    }
}
