using Microsoft.AspNetCore.Identity;

namespace CodeClash.Application.Abstractions.Email;

public interface IEmailService
{
    Task<bool> SendEmailAsync(
        string email,
        string message,
        string? subject);

    Task SendConfirmationEmail(IdentityUser user);
}
