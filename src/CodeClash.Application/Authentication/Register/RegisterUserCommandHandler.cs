using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.DTO;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Authentication.Register;
internal sealed class RegisterUserCommandHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IAuthService identityService,
        ITokenProvider tokenProvider)
    : IRequestHandler<RegisterUserCommand, Result<AccessTokenDto>>
{
    public async Task<Result<AccessTokenDto>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        // 1. Create Identity User
        var identityResult = await identityService.CreateUserAsync(
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

        await transaction.CommitAsync(cancellationToken);

        var tokenRequest = new TokenRequest(identityId, request.Email);

        var accessToken = tokenProvider.Create(tokenRequest);

        // Return Success
        return Result.Success<AccessTokenDto>(accessToken);
    }
}
