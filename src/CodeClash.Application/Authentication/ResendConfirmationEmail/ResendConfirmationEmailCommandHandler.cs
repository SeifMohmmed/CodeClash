using CodeClash.Application.Abstractions.Email;
using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Authentication.ResendConfirmationEmail;

internal sealed class ResendConfirmationEmailCommandHandler(
    IAuthService authService,
    IEmailService emailService)
    : ICommandHandler<ResendConfirmationEmailCommand, string>
{
    public async Task<Result<string>> Handle(
        ResendConfirmationEmailCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Find user by email
        var identityUser = await authService
            .GetUserByEmailAsync(request.Email);

        if (identityUser is null)
        {
            return Result.Failure<string>(UserErrors.NotFound);
        }

        // 2. Check if already confirmed — no need to resend
        if (identityUser.EmailConfirmed)
        {
            return Result.Failure<string>(UserErrors.UserAlreadyConfirmed);
        }

        // 3. Delegate everything (token gen + link building + sending) to EmailService
        try
        {
            await emailService.SendConfirmationEmail(identityUser);
        }
        catch (Exception)
        {
            return Result.Failure<string>(UserErrors.UserEmailFailed);
        }

        return Result.Success<string>("Confirmation email resent successfully.");

    }
}
