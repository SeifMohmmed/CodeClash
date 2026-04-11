using FluentValidation;

namespace CodeClash.Application.Problems.DeleteProblem;
internal sealed class DeleteProblemCommandValidator
    : AbstractValidator<DeleteProblemCommand>
{
    public DeleteProblemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id shouldn't be empty");
    }
}
