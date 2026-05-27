using FluentValidation;

namespace CodeClash.Application.Contests.CreateContest;
public sealed class CreateContestCommandValidator
    : AbstractValidator<CreateContestCommand>
{
    public CreateContestCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().NotNull();
        RuleFor(x => x.Description).NotEmpty().NotNull();
        RuleFor(x => x.StartTime).NotEmpty().NotNull();
        RuleFor(x => x.EndTime).NotEmpty().NotNull();
    }
}
