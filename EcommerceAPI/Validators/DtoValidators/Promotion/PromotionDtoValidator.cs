﻿using FluentValidation;
using Services.DTOs.Promotion;

namespace EcommerceAPI.Validators.DtoValidators.Promotion
{
    public class PromotionDtoValidator : AbstractValidator<PromotionDto>
    {
        public PromotionDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(c => c.Name)
               .NotNull().WithMessage("{PropertyName} must not be null!")
               .NotEmpty().WithMessage("{PropertyName} must not be empty!")
               .Length(1, 150).WithMessage("{PropertyName} must be between 1 and 150 characters!");

            RuleFor(c => c.DiscountAmount)
                .NotNull().WithMessage("{PropertyName} must not be null!")
                .NotEmpty().WithMessage("{PropertyName} must not be empty!")
                .GreaterThan(0).WithMessage("{PropertyName} must be positive!")
                .LessThanOrEqualTo(100).WithMessage("Discount amount must be less than or equal to 100%.");

            RuleFor(x => x.StartDate)
               .GreaterThanOrEqualTo(DateTime.Now.AddDays(-1))
               .WithMessage("Promotion start date must be in the future.");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("Promotion end date must be after the start date.");
        }
    }
}
