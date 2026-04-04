using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.DTO;
using CodeClash.Application.Helpers;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using MediatR;
using Microsoft.Extensions.Options;

namespace CodeClash.Application.Authentication.ConfirmEmail;
internal sealed class ConfirmEmailCommandHandler(
    IUnitOfWork unitOfWork,
    IAuthService authService,
    ITokenProvider tokenProvider,
    IRefreshTokenRepository refreshTokenRepository,
    IOptions<JwtAuthOptions> options)
    : IRequestHandler<ConfirmEmailCommand, Result<AccessTokenDto>>
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

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
            return Result.Failure<AccessTokenDto>(confirmResult.Error);
        }

        var email = confirmResult.Value;

        // 2. Now generate tokens
        var tokenRequest = new TokenRequest(request.UserId, email);

        var accessToken = tokenProvider.Create(tokenRequest);

        var refreshToken = new Domain.Models.Identity.RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = request.UserId,
            Token = accessToken.RefreshToken,
            ExpireAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays)
        };

        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success<AccessTokenDto>(accessToken);
    }
}
