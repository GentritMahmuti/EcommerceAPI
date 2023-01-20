using EcommerceAPI.Models.Entities;
using FluentValidation;

namespace EcommerceAPI.Validators
{
    public class CategoryValidator : AbstractValidator<Category>
    {
        public CategoryValidator()
        {
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

            RuleFor(c => c.CreatedDateTime)
                .Must(BeAValidDate).WithMessage("{PropertyName} cannot be older than year 2000!");
        }

        protected bool BeAValidDate(DateTime date)
        {
            int createdDateTimeYear = date.Year;
            return createdDateTimeYear >= 2000;
        }
    }
}
