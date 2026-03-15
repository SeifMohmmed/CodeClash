using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Authentication.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string UserName) : IRequest<Result<RegisterResponse>>;
