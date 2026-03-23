using CodeClash.Application.DTO;
using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Authentication.RefreshTokens;
public sealed record RefreshTokenCommand(
    string RefreshToken) : IRequest<Result<AccessTokenDto>>;
