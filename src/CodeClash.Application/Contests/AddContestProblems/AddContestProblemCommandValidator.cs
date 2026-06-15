using FluentValidation;

namespace CodeClash.Application.Contests.AddContestProblems;

public sealed class AddContestProblemCommandValidator
    : AbstractValidator<AddContestProblemCommand>
{
    public AddContestProblemCommandValidator()
    {
        RuleFor(x => x.ContestId)
            .NotEmpty().WithMessage("Contest ID is required.");

        RuleFor(x => x.ProblemId)
            .NotEmpty().WithMessage("Problem ID is required.");
    }
}
