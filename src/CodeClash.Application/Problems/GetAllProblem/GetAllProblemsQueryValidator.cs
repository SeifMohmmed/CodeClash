using CodeClash.Application.Problems.GetAll;
using FluentValidation;

namespace CodeClash.Application.Problems.GetAllProblem;
internal sealed class GetAllProblemsQueryValidator
    : AbstractValidator<GetAllProblemsQuery>
{
    public GetAllProblemsQueryValidator()
    {
        RuleFor(x => x.Difficulty)
            .IsInEnum()
            .WithMessage("Invalid difficulty value.");

        RuleFor(x => x.Status)
        .Must(status => !status.HasValue || Enum.IsDefined(status.Value))
        .WithMessage("Status must be either 0 (AC), 1 (Attempted), 2 (Not Attempted), or it can be null.");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page size must be greater than or equal to 1.");

        RuleFor(x => x.TopicsNames)
            .Must(topicsNames => topicsNames == null || topicsNames.Count > 0)
            .WithMessage("Topics names must be null or have at least one element.");

        RuleFor(x => x.Name)
            .Must(problemName => problemName == null || problemName.Length > 0)
            .WithMessage("Problem name must be null or have at least one character.");
    }
}
