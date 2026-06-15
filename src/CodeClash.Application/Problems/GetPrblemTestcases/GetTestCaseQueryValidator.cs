using FluentValidation;

namespace CodeClash.Application.Problems.GetPrblemTestcases;

public sealed class GetTestCaseQueryValidator
    : AbstractValidator<GetTestCaseQuery>
{
    public GetTestCaseQueryValidator()
    {
        RuleFor(x => x.ProblemId)
            .NotEmpty();
    }
}
