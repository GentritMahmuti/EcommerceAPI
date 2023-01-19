using EcommerceAPI.Models.Entities;
using FluentValidation;

namespace EcommerceAPI.Validators
{
    public class OrderDetailsValidator : AbstractValidator<OrderDetails>
    {
        public OrderDetailsValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(c => c.Id)
                .NotNull().WithMessage("{PropertyName} must not be null!")
                .NotEmpty().WithMessage("{PropertyName} must not be empty!")
                .GreaterThan(0).WithMessage("{PropertyName} must be positive");


            RuleFor(c => c.ProductId)
               .NotNull().WithMessage("{PropertyName} must not be null!")
               .NotEmpty().WithMessage("{PropertyName} must not be empty!")
               .GreaterThan(0).WithMessage("{PropertyName} must be positive");

        }
    }
}
