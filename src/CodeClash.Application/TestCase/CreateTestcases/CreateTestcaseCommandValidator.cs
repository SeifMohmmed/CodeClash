using FluentValidation;

namespace CodeClash.Application.TestCase.CreateTestcases;

internal sealed class CreateTestcaseCommandValidator
    : AbstractValidator<CreateTestcaseCommand>
{
    public CreateTestcaseCommandValidator()
    {
        RuleFor(x => x.ProblemId)
            .NotEmpty()
            .WithMessage("ProblemId must be a valid GUID.");

        RuleFor(x => x.Input)
            .NotEmpty()
            .WithMessage("Input is required.");

        RuleFor(x => x.Output)
            .NotEmpty()
            .WithMessage("Expected output is required.");
    }
}
