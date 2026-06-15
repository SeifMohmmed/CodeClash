using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Abstractions.Token;
using CodeClash.Application.DTO;
using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Authentication.Login;

internal sealed class LoginQueryHandler(
    IAuthService identityService,
    ITokenService tokenService)
    : IQueryHandler<LoginQuery, AccessTokenDto>
{
    public async Task<Result<AccessTokenDto>> Handle(
        LoginQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Get user
        var identityUser = await identityService
            .GetUserByEmailAsync(request.Email);

        if (identityUser is null)
        {
            return Result.Failure<AccessTokenDto>(UserErrors.InvalidCredentials);
        }

        // 2. Check password
        var isValid = await identityService
            .CheckPasswordAsync(identityUser, request.Password);

        if (!isValid)
        {
            return Result.Failure<AccessTokenDto>(UserErrors.InvalidCredentials);
        }

        // 3. Generate token
        var accessToken = await tokenService
            .GenerateTokensAsync(identityUser.Id, identityUser.Email!, cancellationToken);

        return Result.Success(accessToken);
    }
}
