using EcommerceAPI.Models.DTOs.Review;
using FluentValidation;

namespace EcommerceAPI.Validators
{
    public class ReviewUpdateDtoValidator : AbstractValidator<ReviewUpdateDto>
    {
        public ReviewUpdateDtoValidator()
        {
            RuleFor(x => x.ReviewId).NotEmpty()
                                    .WithMessage("ProductId is required");

            RuleFor(x => x.Rating).NotEmpty()
                                  .WithMessage("Rating is required")
                                  .InclusiveBetween(1, 10)
                                  .WithMessage("Rating must be between 1 and 10");

            RuleFor(x => x.ReviewComment).NotEmpty()
                                         .WithMessage("ReviewComment is required")
                                         .MaximumLength(200)
                                         .WithMessage("ReviewComment must be 200 characters or less");
        }
    }
}
