using CodeClash.Application.Abstractions.Email;
using CodeClash.Application.Abstractions.Identity;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Authentication.ResendConfirmationEmail;
internal sealed class ResendConfirmationEmailCommandHandler(
    IAuthService authService,
    IEmailService emailService)
    : IRequestHandler<ResendConfirmationEmailCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        ResendConfirmationEmailCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Find user by email
        var identityUser = await authService.GetUserByEmailAsync(request.Email);

        if (identityUser is null)
        {
            return Result.Failure<string>(new Error("Auth.NotFound", "User not found."));
        }

        // 2. Check if already confirmed — no need to resend
        if (identityUser.EmailConfirmed)
        {
            return Result.Failure<string>(new Error("Auth.AlreadyConfirmed", "Email is already confirmed."));
        }

        // 3. Delegate everything (token gen + link building + sending) to EmailService
        try
        {
            await emailService.SendConfirmationEmail(identityUser);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>(new Error("Auth.EmailFailed", $"Failed to send confirmation email: {ex.Message}"));
        }

        return Result.Success<string>("Confirmation email resent successfully.");

    }
}
