using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.DTO;

namespace CodeClash.Application.Authentication.Login;

public sealed record LoginQuery(
    string Email,
    string Password) : IQuery<AccessTokenDto>;
