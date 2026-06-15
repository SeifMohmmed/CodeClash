using FluentValidation;

namespace CodeClash.Application.Contests.UpdateContest;

public sealed class UpdateContestCommandValidator
    : AbstractValidator<UpdateContestCommand>
{
    public UpdateContestCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Contest ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Contest name is required.")
            .MinimumLength(3).WithMessage("Contest name must be at least 3 characters.")
            .MaximumLength(100).WithMessage("Contest name must not exceed 100 characters.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Start date must be in the future.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");
    }
}
