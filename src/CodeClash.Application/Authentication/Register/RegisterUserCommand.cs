using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.DTO;

namespace CodeClash.Application.Authentication.Register;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string Name) : ICommand<RegisterResponseDto>;
