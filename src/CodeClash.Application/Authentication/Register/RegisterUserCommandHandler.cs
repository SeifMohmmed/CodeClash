using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.DTO;
using CodeClash.Application.Helpers;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using MediatR;
using Microsoft.Extensions.Options;

namespace CodeClash.Application.Authentication.Register;
internal sealed class RegisterUserCommandHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IAuthService authService,
        ITokenProvider tokenProvider,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<JwtAuthOptions> options)
    : IRequestHandler<RegisterUserCommand, Result<AccessTokenDto>>
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    public async Task<Result<AccessTokenDto>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Create Identity User
            var identityResult = await authService.CreateUserAsync(
                request.Email,
                request.Password);

            if (identityResult.IsFailure)
            {
                throw new Exception(identityResult.Error.Message);
            }

            var identityId = identityResult.Value;

            // 2. Create Domain User
            var user = request.ToEntity(identityId);
            await userRepository.AddAsync(user);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var tokenRequest = new TokenRequest(identityId, request.Email);

            var accessToken = tokenProvider.Create(tokenRequest);

            var refreshToken = new Domain.Models.Identity.RefreshToken
            {
                Id = Guid.CreateVersion7(),
                UserId = identityId,
                Token = accessToken.RefreshToken,
                ExpireAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays)
            };

            await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            // Return Success
            return Result.Success<AccessTokenDto>(accessToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
