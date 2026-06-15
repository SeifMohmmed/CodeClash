using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Authorization.GetAllRoles;

public sealed record GetAllRolesQuery
    : IQuery<IReadOnlyList<RoleResponse>>;

