using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.Abstractions.Token;
using CodeClash.Application.DTO;
using CodeClash.Application.Helpers;
using CodeClash.Domain.Models.Identity;
using Microsoft.Extensions.Options;

namespace CodeClash.Infrastructure.Implementation;

internal sealed class TokenService(
    ITokenProvider tokenProvider,
    IIdentityDbContext identityDbContext,
    IAuthService identityService,
    IOptions<JwtAuthOptions> options) : ITokenService
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    public async Task<AccessTokenDto> GenerateTokensAsync(
        string userId,
        string email,
        CancellationToken cancellationToken = default)
    {
        var roles = await identityService.GetUserRolesAsync(userId);

        var tokenRequest = new TokenRequest(userId, email, roles);
        var accessToken = tokenProvider.Create(tokenRequest);

        var refreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Token = accessToken.RefreshToken,
            ExpireAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays)
        };

        identityDbContext.RefreshTokens.Add(refreshToken);
        await identityDbContext.SaveChangesAsync(cancellationToken);

        return accessToken;
    }

    public async Task<AccessTokenDto> RotateTokensAsync(
        RefreshToken existingToken,
        CancellationToken cancellationToken = default)
    {
        var roles = await identityService
            .GetUserRolesAsync(existingToken.User.Id);

        var tokenRequest = new TokenRequest(
            existingToken.User.Id,
            existingToken.User.Email!,
            roles);

        var accessToken = tokenProvider.Create(tokenRequest);

        existingToken.Token = accessToken.RefreshToken;
        existingToken.ExpireAtUtc = DateTime.UtcNow
            .AddDays(_jwtAuthOptions.RefreshTokenExpirationDays);

        await identityDbContext.SaveChangesAsync(cancellationToken);

        return accessToken;
    }
}
