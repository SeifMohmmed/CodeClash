using FluentValidation;

namespace CodeClash.Application.Submissions.GetProblemSubmissions;

public sealed class GetProblemSubmissionsQueryValidator
    : AbstractValidator<GetProblemSubmissionsQuery>
{
    public GetProblemSubmissionsQueryValidator()
    {
        RuleFor(x => x.ProblemId)
            .NotEmpty()
            .WithMessage("ProblemId is required.");
    }
}
