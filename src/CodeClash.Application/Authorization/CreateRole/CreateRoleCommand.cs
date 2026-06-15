using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Authorization.CreateRole;

public sealed record CreateRoleCommand(
    string RoleName) : ICommand<RoleResponse>;
