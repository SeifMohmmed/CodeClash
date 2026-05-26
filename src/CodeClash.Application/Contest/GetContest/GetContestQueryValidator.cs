using FluentValidation;

namespace CodeClash.Application.Contest.GetContest;
public sealed class GetContestQueryValidator
    : AbstractValidator<GetContestQuery>
{
    public GetContestQueryValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}
