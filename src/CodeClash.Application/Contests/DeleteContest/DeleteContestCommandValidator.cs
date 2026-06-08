using FluentValidation;

namespace CodeClash.Application.Contests.DeleteContest;

public sealed class DeleteContestCommandValidator
    : AbstractValidator<DeleteContestCommand>
{
    public DeleteContestCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty);
    }
}
