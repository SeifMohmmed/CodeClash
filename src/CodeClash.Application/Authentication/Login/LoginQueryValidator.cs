using FluentValidation;

namespace CodeClash.Application.Authentication.Login;
internal sealed class LoginQueryValidator : AbstractValidator<LoginQuery>
{
    public LoginQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email must not be Empty")
            .NotNull().WithMessage("Email must not be Null")
            .EmailAddress().WithMessage("EmailAddress must not be Empty")
            .MinimumLength(5).WithMessage("MinimumLength must be 5")
            .MaximumLength(50).WithMessage("MaximumLength must be 50");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password must Not be Empty")
            .NotNull().WithMessage("Password must not be Null")
            .MinimumLength(5).WithMessage("MinimumLength must be 5")
            .MaximumLength(50).WithMessage("MaximumLength must be 50");
    }
}
