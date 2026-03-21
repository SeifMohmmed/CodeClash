using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Authentication.Register;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string Name) : IRequest<Result<string>>;
