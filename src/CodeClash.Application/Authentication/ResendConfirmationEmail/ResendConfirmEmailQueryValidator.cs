using FluentValidation;

namespace CodeClash.Application.Authentication.ResendConfirmationEmail;
public sealed class ResendConfirmEmailQueryValidator
    : AbstractValidator<ResendConfirmationEmailCommand>
{
    public ResendConfirmEmailQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email must be Not Empty")
            .NotNull().WithMessage("Email must Not be Null")
            .EmailAddress().WithMessage("Must Be Email");
    }
}
