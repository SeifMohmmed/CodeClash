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
            .NotEmpty().WithMessage("Input is required.")
            .MinimumLength(1).WithMessage("Input must be at least 1 character long.");

        RuleFor(x => x.Output)
            .NotEmpty().WithMessage("Expected output is required.")
            .MinimumLength(1).WithMessage("Output must be at least 1 character long.");
    }
}
