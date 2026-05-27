using FluentValidation;

namespace CodeClash.Application.Contests.GetContest;
public sealed class GetContestQueryValidator
    : AbstractValidator<GetContestQuery>
{
    public GetContestQueryValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}
