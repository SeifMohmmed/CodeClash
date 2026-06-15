using FluentValidation;

namespace CodeClash.Application.Contests.CreateContest;

public sealed class CreateContestCommandValidator
    : AbstractValidator<CreateContestCommand>
{
    public CreateContestCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Contest name is required.")
            .MinimumLength(3).WithMessage("Contest name must be at least 3 characters.")
            .MaximumLength(100).WithMessage("Contest name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Start time must be in the future.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required.")
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");
    }
}
