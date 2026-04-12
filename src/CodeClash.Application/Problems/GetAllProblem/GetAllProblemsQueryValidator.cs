using CodeClash.Application.Problems.GetAll;
using FluentValidation;

namespace CodeClash.Application.Problems.GetAllProblem;
internal sealed class GetAllProblemsQueryValidator
    : AbstractValidator<GetAllProblemsQuery>
{
    public GetAllProblemsQueryValidator()
    {
        RuleFor(x => x.Name).Null().Empty().MinimumLength(5).MaximumLength(30);
        RuleFor(x => x.TopicsIds).Null().Empty();

        RuleFor(x => x.Difficulty)
            .IsInEnum()
            .WithMessage("Invalid difficulty value.");

        RuleFor(x => x.UserId).NotEmpty().NotNull();
    }
}
