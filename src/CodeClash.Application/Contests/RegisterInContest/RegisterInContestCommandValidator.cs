using FluentValidation;

namespace CodeClash.Application.Contests.RegisterInContest;

public class RegisterInContestCommandValidator
    : AbstractValidator<RegisterInContestCommand>
{
    public RegisterInContestCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
