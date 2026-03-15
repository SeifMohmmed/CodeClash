using FluentValidation;

namespace CodeClash.Application.Authentication.Register;
internal sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
             .NotEmpty().WithMessage("Email must not be Empty")
             .NotNull().WithMessage("Email must not be Null")
             .EmailAddress().WithMessage("EmailAddress must not be Empty")
             .MinimumLength(5).WithMessage("MinimumLength must be 5")
             .MaximumLength(50).WithMessage("MaximumLength must be 50");


        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName must not be Empty")
            .NotNull().WithMessage("UserName must not be Null")
            .MinimumLength(5).WithMessage("MinimumLength must be 5")
            .MaximumLength(50).WithMessage("MaximumLength must be 50");



        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password must not be Empty")
            .NotNull().WithMessage("Password must not be Null")
            .MinimumLength(5).WithMessage("Password must be not 5")
            .MaximumLength(50).WithMessage("MaximumLength must 50")
            .WithMessage("Password must be between 5 and 50 characters long.");

    }
}
