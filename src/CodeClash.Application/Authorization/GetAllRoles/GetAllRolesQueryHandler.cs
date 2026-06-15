using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Abstractions.Roles;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Authorization.GetAllRoles;

public sealed class GetAllRolesQueryHandler(
    IRoleService roleService) : IQueryHandler<GetAllRolesQuery, IReadOnlyList<RoleResponse>>
{
    public async Task<Result<IReadOnlyList<RoleResponse>>> Handle(
        GetAllRolesQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await roleService.GetAllRolesAsync();

        return Result.Success<IReadOnlyList<RoleResponse>>(
            roles.Select(r => new RoleResponse(r.Id, r.Name!)).ToList());
    }
}
