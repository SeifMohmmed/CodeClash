using FluentValidation;

namespace CodeClash.Application.Emails.SendEmails;

public sealed class SendEmailCommandValidator
    : AbstractValidator<SendEmailCommand>
{
    public SendEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MaximumLength(5000).WithMessage("Message must not exceed 5000 characters.");

        RuleFor(x => x.Subject)
            .MaximumLength(200).WithMessage("Subject must not exceed 200 characters.")
            .When(x => x.Subject is not null);
    }
}
