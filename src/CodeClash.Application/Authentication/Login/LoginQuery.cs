using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Authentication.Login;
public sealed record LoginQuery(
    string Email,
    string Password) : IRequest<Result<LoginResponse>>;
