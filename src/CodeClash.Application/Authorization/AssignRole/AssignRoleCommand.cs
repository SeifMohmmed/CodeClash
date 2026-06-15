using CodeClash.Application.Abstractions.Messaging;
using MediatR;

namespace CodeClash.Application.Authorization.AssignRole;

public sealed record AssignRoleCommand(
    string UserId,
    string RoleName) : ICommand<Unit>;
