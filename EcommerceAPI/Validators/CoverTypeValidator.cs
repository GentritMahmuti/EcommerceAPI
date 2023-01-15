using EcommerceAPI.Models.Entities;
using FluentValidation;

namespace EcommerceAPI.Validators
{
    public class CoverTypeValidator : AbstractValidator<CoverType>
    {
        public CoverTypeValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(c => c.Id)
                .NotNull().WithMessage("{PropertyName} must not be null!")
                .NotEmpty().WithMessage("{PropertyName} must not be empty!")
                .GreaterThan(0).WithMessage("{PropertyName} must be positive");

            RuleFor(c => c.Name)
                .NotNull().WithMessage("{PropertyName} must not be null!")
                .NotEmpty().WithMessage("{PropertyName} must not be empty!")
                .Length(1, 100).WithMessage("{PropertyName} must be between 1 and 100 characters!");
        }
    }
}
