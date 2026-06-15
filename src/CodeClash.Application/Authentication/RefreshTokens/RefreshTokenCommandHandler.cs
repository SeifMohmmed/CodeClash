using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.Abstractions.Token;
using CodeClash.Application.DTO;
using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Premitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.Application.Authentication.RefreshTokens;

public sealed class RefreshTokenCommandHandler(
    IIdentityDbContext identityDbContext,
    ITokenService tokenService) : IRequestHandler<RefreshTokenCommand, Result<AccessTokenDto>>
{
    public async Task<Result<AccessTokenDto>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var refreshToken = await identityDbContext.RefreshTokens
                  .Include(rt => rt.User)
                  .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (refreshToken is null)
        {
            return Result.Failure<AccessTokenDto>(RefreshTokenErrors.Invalid);
        }

        if (refreshToken.ExpireAtUtc < DateTime.UtcNow)
        {
            return Result.Failure<AccessTokenDto>(RefreshTokenErrors.Expired);
        }

        var accessToken = await tokenService
            .RotateTokensAsync(refreshToken, cancellationToken);

        return Result.Success(accessToken);
    }
}
