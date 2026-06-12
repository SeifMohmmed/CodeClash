using FluentValidation;

namespace CodeClash.Application.Contests.UpdateContest;

public sealed class UpdateContestCommandValidator
    : AbstractValidator<UpdateContestCommand>
{
    public UpdateContestCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.StartDate)
            .NotEmpty();

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date");
    }
}
