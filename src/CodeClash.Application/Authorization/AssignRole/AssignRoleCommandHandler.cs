using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Abstractions.Roles;
using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Authorization.AssignRole;

internal sealed class AssignRoleCommandHandler(
    IRoleService roleService) : ICommandHandler<AssignRoleCommand, Unit>
{
    public async Task<Result<Unit>> Handle(
        AssignRoleCommand request,
        CancellationToken cancellationToken)
    {
        await roleService.AssignRoleAsync(request.UserId, request.RoleName);

        return Result.Success(Unit.Value);
    }
}
