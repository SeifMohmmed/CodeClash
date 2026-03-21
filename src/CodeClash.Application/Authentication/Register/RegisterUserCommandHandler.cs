using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Authentication.Register;
internal sealed class RegisterUserCommandHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IIdentityService identityService)
    : IRequestHandler<RegisterUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var identityResult = await identityService.CreateUserAsync(
            request.Email,
            request.Password);

        if (identityResult.IsFailure)
        {
            return Result.Failure<string>(identityResult.Error);

        }

        var identityId = identityResult.Value;

        // Create Domain User
        var user = request.ToEntity(identityId);

        await userRepository.AddAsync(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Return Success
        return Result.Success<string>(user.Id);
    }
}
