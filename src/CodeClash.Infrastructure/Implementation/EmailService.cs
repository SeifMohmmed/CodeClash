using CodeClash.Application.Abstractions.Email;
using CodeClash.Application.Helpers;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CodeClash.Infrastructure.Implementation;
internal sealed class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task<bool> SendEmailAsync(
        string email,
        string _message,
        string? reason)
    {
        try
        {
            using var client = new SmtpClient();

            await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(_emailSettings.FromEmail, _emailSettings.Password);

            var bodybuilder = new BodyBuilder
            {
                HtmlBody = $"{_message}",
                TextBody = "welcome",
            };

            using var message = new MimeMessage
            {
                Body = bodybuilder.ToMessageBody()
            };

            message.From.Add(new MailboxAddress("Code Clash", _emailSettings.FromEmail));
            message.To.Add(new MailboxAddress("Dev", email));
            message.Subject = reason == null ? "" : reason;
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EMAIL ERROR: {ex.Message}");
            throw;
        }
    }
}

