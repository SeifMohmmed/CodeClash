using CodeClash.Application.Abstractions.Email;
using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.DTO;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Premitives;
using Microsoft.Extensions.Logging;

namespace CodeClash.Application.Authentication.Register;

internal sealed class RegisterUserCommandHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IAuthService authService,
        IEmailService emailService,
        ILogger<RegisterUserCommandHandler> logger)
    : ICommandHandler<RegisterUserCommand, RegisterResponseDto>
{
    public async Task<Result<RegisterResponseDto>> Handle(
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
                return Result.Failure<RegisterResponseDto>(UserErrors.NotFound);
            }

            var identityId = identityResult.Value;

            // 2. Create Domain User
            var user = request.ToEntity(identityId);

            await userRepository.AddAsync(user);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            // 3. Fetch Identity User to generate token
            var identityUser = await authService.GetUserByIdAsync(identityId);

            if (identityUser is null)
            {
                return Result.Failure<RegisterResponseDto>(UserErrors.NotFound);
            }

            // 4. Commit BEFORE sending email
            //    so a failed email doesn't roll back the created user
            await transaction.CommitAsync(cancellationToken);

            // 5. Send confirmation email — outside the transaction
            //    if it fails, user can resend via /resend-confirmation-email
            try
            {
                await emailService.SendConfirmationEmail(identityUser);
            }
            catch (Exception ex)
            {
                // User is created successfully, email just failed
                // Log it, but don't fail the whole registration
                logger.LogWarning(
                    ex,
                    "Confirmation email failed for {Email}: {Error}",
                    request.Email, ex.Message);
            }

            return Result.Success<RegisterResponseDto>(
                new RegisterResponseDto(
                "Registration successful. Please confirm your email before logging in.",
                request.Email));
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
