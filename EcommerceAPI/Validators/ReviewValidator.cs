using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.DTOs.Review;
using EcommerceAPI.Services.IServices;
using FluentValidation;


namespace EcommerceAPI.Validators
{
    public class ReviewValidator : AbstractValidator<ReviewCreateDto>
    {
        public ReviewValidator()
        {
           

            RuleFor(c => c.Rating)
                .NotEmpty()
                .WithMessage("Rating can't be empty!")
                .DependentRules(() =>
                {
                    RuleFor(c => c.Rating)
                        .Must(c => c >= 0 && c<=10)
                        .WithMessage("Rating must be between 1-10");
                });

            RuleFor(c => c.ReviewComment)
               .NotEmpty()
               .WithMessage("Review comment can't be empty!")
               .NotNull()
               .WithMessage("Review comment cant be null")
               .DependentRules(() =>
               {
                   RuleFor(c => c.ReviewComment)
                      .Must(c => c.Count() < 500)
                      .WithMessage("Length of review comment can't exceed 500 characters!");
               });

        }
    }

}
