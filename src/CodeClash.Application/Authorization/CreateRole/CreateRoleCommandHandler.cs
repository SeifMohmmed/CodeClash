using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Abstractions.Roles;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Authorization.CreateRole;

internal sealed class CreateRoleCommandHandler(
    IRoleService roleService) : ICommandHandler<CreateRoleCommand, RoleResponse>
{
    public async Task<Result<RoleResponse>> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = await roleService
            .CreateRoleAsync(request.RoleName);

        return Result.Success(new RoleResponse(role.Id, role.Name));
    }
}
