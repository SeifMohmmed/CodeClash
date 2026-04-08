using FluentValidation;

namespace CodeClash.Application.TestCase.DeleteTestcases;
public sealed class DeleteTestcaseCommandValidator
    : AbstractValidator<DeleteTestcaseCommand>
{
    public DeleteTestcaseCommandValidator()
    {
        RuleFor(x => x.TestcaseId)
            .NotEmpty().WithMessage("TestcaseId is required.");
    }
}
