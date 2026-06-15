using FluentValidation;

namespace CodeClash.Application.Plagiarism.GetContestPlagiarismCases;

public sealed class GetContestPlagiarismCasesQueryValidator
    : AbstractValidator<GetContestPlagiarismCasesQuery>
{
    public GetContestPlagiarismCasesQueryValidator()
    {
        RuleFor(x => x.ContestId)
            .NotEmpty().WithMessage("Contest ID is required.");

        RuleFor(x => x.Threshold)
            .InclusiveBetween(0, 1)
            .WithMessage("Threshold must be between 0 and 1.");

        RuleFor(x => x.ProblemIds)
            .NotEmpty().WithMessage("At least one problem ID is required.")
            .Must(ids => ids.All(id => id != Guid.Empty))
            .WithMessage("Problem IDs must not contain empty GUIDs.");
    }
}
