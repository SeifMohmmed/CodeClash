using FluentValidation;

namespace CodeClash.Application.Problems.DeleteProblem;

public sealed class DeleteProblemCommandValidator
    : AbstractValidator<DeleteProblemCommand>
{
    public DeleteProblemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
