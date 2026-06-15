using FluentValidation;

namespace CodeClash.Application.Authorization.AssignRole;

public sealed class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
{
    public AssignRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.RoleName)
            .NotEmpty().WithMessage("Role name is required.")
            .MinimumLength(2).WithMessage("Role name must be at least 2 characters.")
            .MaximumLength(50).WithMessage("Role name must not exceed 50 characters.")
            .Matches("^[a-zA-Z]+$").WithMessage("Role name must contain only letters.");
    }
}
