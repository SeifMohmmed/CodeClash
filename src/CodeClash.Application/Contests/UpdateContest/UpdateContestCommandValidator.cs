using FluentValidation;

namespace CodeClash.Application.Contests.UpdateContest;

public sealed class UpdateContestCommandValidator
    : AbstractValidator<UpdateContestCommand>
{
    public UpdateContestCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .NotNull();
    }
}
