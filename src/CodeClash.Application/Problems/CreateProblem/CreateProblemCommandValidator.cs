using FluentValidation;

namespace CodeClash.Application.Problems.CreateProblem;

internal sealed class CreateProblemCommandValidator
    : AbstractValidator<CreateProblemCommand>
{
    public CreateProblemCommandValidator()
    {
        RuleFor(x => x.ContestId)
    .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.Difficulty)
            .IsInEnum();

        RuleFor(x => x.MemoryLimit)
            .NotNull();

        RuleFor(x => x.RunTimeLimit)
            .GreaterThan(0);

        RuleFor(x => x.Topics)
            .NotNull()
            .Must(topics => topics.Count > 0)
            .WithMessage("At least one topic must be specified.");

        RuleForEach(x => x.Topics)
            .NotEmpty();

        RuleFor(x => x.SetterId)
            .NotEmpty();
    }
}
