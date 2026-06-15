using FluentValidation;

namespace CodeClash.Application.TestCase.UpdateTestcases;

internal sealed class UpdateTestcaseCommandValidator
    : AbstractValidator<UpdateTestcaseCommand>
{
    public UpdateTestcaseCommandValidator()
    {
        RuleFor(x => x.TestcaseId)
            .NotEmpty().WithMessage("TestcaseId is required.");

        RuleFor(x => x.Input)
            .NotEmpty().WithMessage("Input is required.");

        RuleFor(x => x.Output)
            .NotEmpty().WithMessage("Expected output is required.");
    }
}
