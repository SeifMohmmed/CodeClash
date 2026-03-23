using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.DTO;
using CodeClash.Application.Helpers;
using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Premitives;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CodeClash.Application.Authentication.RefreshTokens;
public sealed class RefreshTokenCommandHandler(
    IIdentityDbContext identityDbContext,
    ITokenProvider tokenProvider,
    IOptions<JwtAuthOptions> options) : IRequestHandler<RefreshTokenCommand, Result<AccessTokenDto>>
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

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

        var tokenRequest = new TokenRequest(
            refreshToken.User.Id,
            refreshToken.User.Email!);

        var accessToken = tokenProvider.Create(tokenRequest);

        // Update refresh token
        refreshToken.Token = accessToken.RefreshToken;
        refreshToken.ExpireAtUtc = DateTime.UtcNow.AddDays(
            _jwtAuthOptions.RefreshTokenExiprationDays);

        await identityDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(accessToken);
    }
}
