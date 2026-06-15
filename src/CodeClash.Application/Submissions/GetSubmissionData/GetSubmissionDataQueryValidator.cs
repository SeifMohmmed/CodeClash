using FluentValidation;

namespace CodeClash.Application.Submissions.GetSubmissionData;

public sealed class GetSubmissionDataQueryValidator
    : AbstractValidator<GetSubmissionDataQuery>
{
    public GetSubmissionDataQueryValidator()
    {
        RuleFor(x => x.SubmissionId)
            .NotEmpty()
            .WithMessage("SubmissionId is required.");
    }
}
