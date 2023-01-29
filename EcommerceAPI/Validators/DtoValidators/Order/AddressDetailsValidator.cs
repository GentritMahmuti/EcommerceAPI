using EcommerceAPI.Models.DTOs.Order;
using FluentValidation;
using System.Text.RegularExpressions;

namespace EcommerceAPI.Validators.DtoValidators.Order
{
    public class AddressDetailsValidator : AbstractValidator<AddressDetails>
    {
        public AddressDetailsValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(c => c.PhoheNumber)
                .NotNull().WithMessage("{PropertyName} must not be null!")
                .NotEmpty().WithMessage("{PropertyName} must not be empty!");

            RuleFor(c => c.StreetAddress)
               .NotNull().WithMessage("{PropertyName} must not be null!")
               .NotEmpty().WithMessage("{PropertyName} must not be empty!");

            RuleFor(c => c.City)
              .NotNull().WithMessage("{PropertyName} must not be null!")
              .NotEmpty().WithMessage("{PropertyName} must not be empty!");

            RuleFor(c => c.Country)
             .NotNull().WithMessage("{PropertyName} must not be null!")
             .NotEmpty().WithMessage("{PropertyName} must not be empty!");

            RuleFor(c => c.PostalCode)
             .NotNull().WithMessage("{PropertyName} must not be null!")
             .NotEmpty().WithMessage("{PropertyName} must not be empty!");

            RuleFor(c => c.Name)
                .Length(1, 100).WithMessage("{PropertyName} must be between 1 and 100 characters!")
                .Matches(@"^[A-Za-z\s]*$").WithMessage("{PropertyName} contains invalid characters!");
        }
    }
}
