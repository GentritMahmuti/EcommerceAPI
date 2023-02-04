using FluentValidation;
using Services.DTOs.Review;

namespace EcommerceAPI.Validators.DtoValidators.Review
{
    public class ReviewUpdateDtoValidator : AbstractValidator<ReviewUpdateDto>
    {
        public ReviewUpdateDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(c => c.ReviewId)
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
