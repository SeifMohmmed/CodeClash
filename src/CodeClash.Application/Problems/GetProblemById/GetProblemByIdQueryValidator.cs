using FluentValidation;

namespace CodeClash.Application.Problems.GetProblemById;
internal sealed class GetProblemByIdQueryValidator
    : AbstractValidator<GetProblemByIdQuery>
{
    public GetProblemByIdQueryValidator()
    {
        RuleFor(x => x.ProblemId)
            .NotEmpty()
            .NotNull();
    }
}
