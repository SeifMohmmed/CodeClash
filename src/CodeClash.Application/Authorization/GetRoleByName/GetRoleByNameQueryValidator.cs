using FluentValidation;

namespace CodeClash.Application.Authorization.GetRoleByName;

public sealed class GetRoleByNameQueryValidator : AbstractValidator<GetRoleByNameQuery>
{
    public GetRoleByNameQueryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name is required.")
            .MinimumLength(2).WithMessage("Role name must be at least 2 characters.")
            .MaximumLength(50).WithMessage("Role name must not exceed 50 characters.")
            .Matches("^[a-zA-Z]+$").WithMessage("Role name must contain only letters.");
    }
}
