using FluentValidation;

namespace CodeClash.Application.Problems.CreateProblem;
internal sealed class CreateProblemCommandValidator : AbstractValidator<CreateProblemCommand>
{
    public CreateProblemCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().NotNull().MinimumLength(5).MaximumLength(30);

        RuleFor(p => p.Description).NotEmpty().NotNull().MinimumLength(10).MaximumLength(300);

        RuleFor(p => p.Rate).NotEmpty().NotNull();
    }
}
