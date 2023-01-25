using EcommerceAPI.Models.DTOs.Promotion;
using EcommerceAPI.Models.Entities;
using FluentValidation;

namespace EcommerceAPI.Validators
{
    public class PromotionValidator : AbstractValidator<PromotionDto>
    {
        public PromotionValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Promotion name is required.")
                .MaximumLength(50)
                .WithMessage("Promotion name must not exceed 50 characters.");

            RuleFor(x => x.StartDate)
                .GreaterThanOrEqualTo(DateTime.Now)
                .WithMessage("Promotion start date must be in the future.");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("Promotion end date must be after the start date.");

            RuleFor(x => x.DiscountAmount)
                .GreaterThan(0)
                .WithMessage("Discount amount must be greater than 0.")
                .LessThanOrEqualTo(100)
                .WithMessage("Discount amount must be less than or equal to 100%.");
        }
    }
}
