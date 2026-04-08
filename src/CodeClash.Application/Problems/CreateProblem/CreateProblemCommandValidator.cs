using FluentValidation;

namespace CodeClash.Application.Problems.CreateProblem;
internal sealed class CreateProblemCommandValidator
    : AbstractValidator<CreateProblemCommand>
{
    public CreateProblemCommandValidator()
    {
        RuleFor(x => x.Difficulty).NotEmpty().NotNull();
        RuleFor(x => x.ContestId).NotEmpty().NotNull();
        RuleFor(x => x.RunTimeLimit).NotEmpty().NotNull();
        RuleFor(x => x.MemoryLimit).NotEmpty().NotNull();
    }
}
