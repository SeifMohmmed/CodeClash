namespace CodeClash.Application.Abstractions.Email;
public interface IEmailService
{
    Task<bool> SendEmailAsync(string email, string _message, string? reason);
}
