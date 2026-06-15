using FluentValidation;

namespace CodeClash.Application.Contests.GetContestStanding;

public sealed class GetContestStandingQueryValidator
    : AbstractValidator<GetContestStandingQuery>
{
    public GetContestStandingQueryValidator()
    {
        RuleFor(x => x.ContestId)
            .NotEmpty().WithMessage("Contest ID is required.");

        RuleFor(x => x.Start)
            .GreaterThanOrEqualTo(0).WithMessage("Start must be greater than or equal to 0.");

        RuleFor(x => x.Stop)
            .GreaterThan(x => x.Start).WithMessage("Stop must be greater than Start.")
            .LessThanOrEqualTo(100).WithMessage("Stop must not exceed 100.");
    }
}
