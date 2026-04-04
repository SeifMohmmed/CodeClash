using CodeClash.Application.DTO;
using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Authentication.ConfirmEmail;
public record ConfirmEmailCommand(string UserId, string Token)
    : IRequest<Result<AccessTokenDto>>;
