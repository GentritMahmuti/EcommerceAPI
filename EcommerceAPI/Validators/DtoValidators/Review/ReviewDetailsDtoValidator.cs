using EcommerceAPI.Models.DTOs.Review;
using FluentValidation;

namespace EcommerceAPI.Validators.DtoValidators.Review
{
    public class ReviewDetailsDtoValidator : AbstractValidator<ReviewDetailsDto>
    {
        public ReviewDetailsDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(c => c.UserId)
               .NotNull().WithMessage("{PropertyName} must not be null!")
               .NotEmpty().WithMessage("{PropertyName} must not be empty!");

            RuleFor(c => c.ProductId)
                .NotNull().WithMessage("{PropertyName} must not be null!")
                .NotEmpty().WithMessage("{PropertyName} must not be empty!")
                .GreaterThan(0).WithMessage("{PropertyName} must be positive!");

            RuleFor(c => c.Rating)
               .NotNull().WithMessage("{PropertyName} must not be null!")
               .NotEmpty().WithMessage("{PropertyName} must not be empty!")
               .ExclusiveBetween(1, 10).WithMessage("{PropertyName} must be between 1 and 10!");

        }
    }
}
