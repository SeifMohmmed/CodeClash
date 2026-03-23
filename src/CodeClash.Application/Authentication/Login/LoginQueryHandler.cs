using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.DTO;
using CodeClash.Application.Helpers;
using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Premitives;
using MediatR;
using Microsoft.Extensions.Options;

namespace CodeClash.Application.Authentication.Login;
internal sealed class LoginQueryHandler(
    IAuthService identityService,
    ITokenProvider tokenProvider,
    IOptions<JwtAuthOptions> options,
    IIdentityDbContext identityDbContext)
    : IRequestHandler<LoginQuery, Result<AccessTokenDto>>
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    public async Task<Result<AccessTokenDto>> Handle(
        LoginQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Get user
        var identityUser =
            await identityService.GetUserByEmailAsync(request.Email);

        if (identityUser is null)
        {
            return Result.Failure<AccessTokenDto>(UserErrors.InvalidCredentials);
        }

        // 2. Check password
        var isValid = await identityService.CheckPasswordAsync(
            identityUser,
            request.Password);

        if (!isValid)
        {
            return Result.Failure<AccessTokenDto>(UserErrors.InvalidCredentials);
        }

        // 3. Generate token
        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email!);
        var accessToken = tokenProvider.Create(tokenRequest);

        var refreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = identityUser.Id,
            Token = accessToken.RefreshToken,
            ExpireAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays)
        };

        identityDbContext.RefreshTokens.Add(refreshToken);

        await identityDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success<AccessTokenDto>(accessToken);
    }
}
