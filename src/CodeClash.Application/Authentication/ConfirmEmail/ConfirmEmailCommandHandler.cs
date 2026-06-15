using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Abstractions.Token;
using CodeClash.Application.DTO;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Authentication.ConfirmEmail;

internal sealed class ConfirmEmailCommandHandler(
    IAuthService authService,
    ITokenService tokenService)
    : ICommandHandler<ConfirmEmailCommand, AccessTokenDto>
{
    public async Task<Result<AccessTokenDto>> Handle(
        ConfirmEmailCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Confirm the email via Identity
        var confirmResult = await authService.ConfirmEmailAsync(
            request.UserId,
            request.Token);

        if (confirmResult.IsFailure)
        {
            return Result.Failure<AccessTokenDto>(confirmResult.Error!);
        }

        // 2. Now generate tokens
        var accessToken = await tokenService.GenerateTokensAsync(
            request.UserId,
            confirmResult.Value,
            cancellationToken);

        return Result.Success(accessToken);
    }
}
