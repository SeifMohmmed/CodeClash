using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.DTO;

namespace CodeClash.Application.Authentication.ConfirmEmail;

public record ConfirmEmailCommand(
    string UserId, string Token) : ICommand<AccessTokenDto>;
