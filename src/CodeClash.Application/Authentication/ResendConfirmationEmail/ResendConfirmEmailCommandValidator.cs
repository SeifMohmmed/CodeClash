using FluentValidation;

namespace CodeClash.Application.Authentication.ResendConfirmationEmail;

public sealed class ResendConfirmEmailCommandValidator
    : AbstractValidator<ResendConfirmationEmailCommand>
{
    public ResendConfirmEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(50).WithMessage("Email must not exceed 50 characters.");
    }
}
