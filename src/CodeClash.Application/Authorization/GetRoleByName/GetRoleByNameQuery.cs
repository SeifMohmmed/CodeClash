using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Authorization.GetRoleByName;

public sealed record GetRoleByNameQuery(
    string Name) : IQuery<RoleResponse>;
