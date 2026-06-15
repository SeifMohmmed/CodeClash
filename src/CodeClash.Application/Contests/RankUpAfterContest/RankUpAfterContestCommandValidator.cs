using FluentValidation;

namespace CodeClash.Application.Contests.RankUpAfterContest;

public sealed class RankUpAfterContestCommandValidator : AbstractValidator<RankUpAfterContestCommand>
{
    public RankUpAfterContestCommandValidator()
    {
        RuleFor(x => x.ContestId)
            .NotEmpty().WithMessage("Contest ID is required.");
    }
}
