using CodeClash.Application.Problems.GetAll;
using FluentValidation;

namespace CodeClash.Application.Problems.GetAllProblem;

internal sealed class GetAllProblemsQueryValidator
    : AbstractValidator<GetAllProblemsQuery>
{
    public GetAllProblemsQueryValidator()
    {
        RuleFor(x => x.Topics)
            .ForEach(topic => topic.NotEmpty())
            .When(x => x.Topics is not null);

        RuleFor(x => x.Name)
            .MaximumLength(200)
            .When(x => x.Name is not null);

        RuleFor(x => x.Difficulty)
            .IsInEnum()
            .When(x => x.Difficulty is not null);

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status is not null);

        RuleFor(x => x.SortBy)
            .IsInEnum();

        RuleFor(x => x.Order)
            .IsInEnum();

        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
