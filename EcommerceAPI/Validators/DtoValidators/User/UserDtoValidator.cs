using EcommerceAPI.Models.DTOs.User;
using FluentValidation;

namespace EcommerceAPI.Validators.DtoValidators.User
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
            RuleFor(x => x.FirsName).NotEmpty().WithMessage("FirstName is required")
                                     .MaximumLength(50).WithMessage("FirstName cannot be more than 50 characters");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName is required")
                                    .MaximumLength(50).WithMessage("LastName cannot be more than 50 characters");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
                                .EmailAddress().WithMessage("Invalid email address")
                                .MaximumLength(100).WithMessage("Email cannot be more than 100 characters");
            RuleFor(x => x.DateOfBirth).NotNull().WithMessage("DateOfBirth is required");
            RuleFor(x => x.Gender).NotEmpty().WithMessage("Gender is required");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("PhoneNumber is required")
                                       .MaximumLength(15).WithMessage("PhoneNumber cannot be more than 15 characters");
        }
    }

}
