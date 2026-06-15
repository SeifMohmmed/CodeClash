using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Abstractions.Roles;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Authorization.GetRoleByName;

internal sealed class GetRoleByNameQueryHandler(
    IRoleService roleService) : IQueryHandler<GetRoleByNameQuery, RoleResponse>
{
    public async Task<Result<RoleResponse>> Handle(
        GetRoleByNameQuery request,
        CancellationToken cancellationToken)
    {
        var role = await roleService.GetRoleByNameAsync(request.Name);

        if (role is null)
        {
            return Result.Failure<RoleResponse>(
                new Error("Role.NotFound", $"Role '{request.Name}' was not found."));
        }

        return Result.Success(new RoleResponse(role.Id, role.Name!));
    }
}
