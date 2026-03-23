using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.DTO;
using CodeClash.Application.Helpers;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace CodeClash.Application.Authentication.Register;
internal sealed class RegisterUserCommandHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IAuthService authService,
        ITokenProvider tokenProvider,
        IIdentityDbContext identityDbContext,
        IAppDbContext appDbContext,
        IOptions<JwtAuthOptions> options)
    : IRequestHandler<RegisterUserCommand, Result<AccessTokenDto>>
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    public async Task<Result<AccessTokenDto>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        await using var transaction = await identityDbContext
           .Database
           .BeginTransactionAsync(cancellationToken);

        appDbContext.Database.SetDbConnection(
            identityDbContext.Database.GetDbConnection());

        await appDbContext.Database.UseTransactionAsync(
            transaction.GetDbTransaction(),
            cancellationToken);

        // 1. Create Identity User
        var identityResult = await authService.CreateUserAsync(
            request.Email,
            request.Password);

        if (identityResult.IsFailure)
        {
            await transaction.RollbackAsync(cancellationToken);

            return Result.Failure<AccessTokenDto>(identityResult.Error);

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
            ExpireAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExiprationDays)
        };

        identityDbContext.RefreshTokens.Add(refreshToken);

        await identityDbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        // Return Success
        return Result.Success<AccessTokenDto>(accessToken);
    }
}
