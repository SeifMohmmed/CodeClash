using FluentValidation;

namespace CodeClash.Application.SolveProblem.SubmitSolutions;

public sealed class SubmitSolutionCommandValidator
    : AbstractValidator<SubmitSolutionCommand>
{
    private const long MaxFileSizeInBytes = 1 * 1024 * 1024; // 1 MB
    public SubmitSolutionCommandValidator()
    {
        RuleFor(x => x.ProblemId)
            .NotEmpty()
            .WithMessage("ProblemId is required.");

        RuleFor(x => x.Code)
            .NotNull()
            .WithMessage("Code file is required.")
            .Must(file => file.Length > 0)
            .WithMessage("Code file must not be empty.")
            .Must(file => file.Length <= MaxFileSizeInBytes)
            .WithMessage("Code file must not exceed 1 MB.")
            .When(x => x.Code is not null);

        RuleFor(x => x.ContestId)
            .NotEmpty()
            .WithMessage("ContestId, if provided, must not be empty.")
            .When(x => x.ContestId.HasValue);

        RuleFor(x => x.Language)
            .IsInEnum()
            .WithMessage("Invalid language value. Must be a defined enum.");
    }
}
