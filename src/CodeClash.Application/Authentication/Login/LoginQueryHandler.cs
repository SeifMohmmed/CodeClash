using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.DTO;
using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Authentication.Login;
internal sealed class LoginQueryHandler(
    IAuthService identityService,
    ITokenProvider tokenProvider)
    : IRequestHandler<LoginQuery, Result<AccessTokenDto>>
{
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

        return Result.Success<AccessTokenDto>(accessToken);
    }
}
