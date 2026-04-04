using System.Text;
using CodeClash.Application.Abstractions.Email;
using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.Helpers;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CodeClash.Infrastructure.Implementation;
internal sealed class EmailService(
    IAuthService authService,
    IHttpContextAccessor httpContextAccessor,
    IOptions<EmailSettings> emailSettings) : IEmailService
{
    private readonly EmailSettings _emailSettings = emailSettings.Value;

    public async Task SendConfirmationEmail(IdentityUser user)
    {
        var code = await authService.GenerateEmailConfirmationTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var origin = httpContextAccessor.HttpContext?.Request.Headers["Host"];

        string link = $"https://{origin}/auth/confirm-email?userId={user.Id}&token={code}";

        string htmlMessage = $"""
    <!DOCTYPE html>
    <html>
    <head>
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
    </head>
    <body style="margin:0; padding:0; background-color:#f4f4f4; font-family:-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; -webkit-font-smoothing:antialiased;">

        <!-- Outer wrapper -->
        <table width="100%" cellpadding="0" cellspacing="0" style="background-color:#f4f4f4; padding:40px 0;">
            <tr>
                <td align="center">

                    <!-- Card -->
                    <table width="600" cellpadding="0" cellspacing="0" style="background-color:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 4px 16px rgba(0,0,0,0.08); max-width:600px;">

                        <!-- Header -->
                        <tr>
                            <td style="background: linear-gradient(135deg, #4F46E5 0%, #7C3AED 100%); padding:40px 32px; text-align:center;">
                                <h1 style="color:#ffffff; margin:0; font-size:32px; font-weight:800; letter-spacing:-0.5px;">
                                    ⚔️ Code Clash
                                </h1>
                                <p style="color:rgba(255,255,255,0.75); margin:8px 0 0 0; font-size:14px; letter-spacing:1px; text-transform:uppercase;">
                                    Email Confirmation
                                </p>
                            </td>
                        </tr>

                        <!-- Body -->
                        <tr>
                            <td style="padding:48px 40px 32px 40px;">

                                <h2 style="color:#111827; margin:0 0 16px 0; font-size:24px; font-weight:700; line-height:1.3;">
                                    Confirm your email address
                                </h2>

                                <p style="color:#6B7280; font-size:16px; line-height:1.7; margin:0 0 32px 0;">
                                    Thanks for joining <strong style="color:#111827;">Code Clash</strong>! 
                                    You're one step away from competing. Please confirm your email address 
                                    by clicking the button below.
                                </p>

                                <!-- Info box -->
                                <table width="100%" cellpadding="0" cellspacing="0" style="background-color:#F5F3FF; border-left:4px solid #4F46E5; border-radius:0 8px 8px 0; margin:0 0 32px 0;">
                                    <tr>
                                        <td style="padding:16px 20px;">
                                            <p style="margin:0; color:#5B21B6; font-size:14px; line-height:1.6;">
                                                ⏱️ This confirmation link will expire in <strong>24 hours</strong>.
                                                After that, you'll need to request a new one.
                                            </p>
                                        </td>
                                    </tr>
                                </table>

                                <!-- CTA Button -->
                                <table cellpadding="0" cellspacing="0" style="margin:0 0 32px 0;">
                                    <tr>
                                        <td style="background: linear-gradient(135deg, #4F46E5 0%, #7C3AED 100%); border-radius:8px;">
                                            <a href="{link}"
                                               style="display:inline-block; padding:16px 40px; color:#ffffff; text-decoration:none; font-size:16px; font-weight:700; letter-spacing:0.3px;">
                                                ✅ Confirm My Email
                                            </a>
                                        </td>
                                    </tr>
                                </table>

                                <!-- Divider -->
                                <table width="100%" cellpadding="0" cellspacing="0" style="margin:0 0 24px 0;">
                                    <tr>
                                        <td style="border-top:1px solid #E5E7EB;"></td>
                                    </tr>
                                </table>

                                <!-- Fallback link -->
                                <p style="color:#9CA3AF; font-size:13px; margin:0 0 8px 0;">
                                    Button not working? Copy and paste this link into your browser:
                                </p>
                                <p style="font-size:12px; word-break:break-all; margin:0 0 32px 0;">
                                    <a href="{link}" style="color:#4F46E5; text-decoration:underline;">{link}</a>
                                </p>

                                <!-- Warning -->
                                <p style="color:#9CA3AF; font-size:13px; margin:0; line-height:1.6;">
                                    🔒 If you didn't create a Code Clash account, you can safely ignore this email.
                                    Someone may have entered your email address by mistake.
                                </p>

                            </td>
                        </tr>

                        <!-- Footer -->
                        <tr>
                            <td style="background-color:#F9FAFB; padding:24px 40px; border-top:1px solid #E5E7EB;">
                                <table width="100%" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td align="center">
                                            <p style="color:#111827; font-size:16px; font-weight:700; margin:0 0 4px 0;">
                                                ⚔️ Code Clash
                                            </p>
                                            <p style="color:#9CA3AF; font-size:12px; margin:0 0 12px 0;">
                                                Compete. Code. Conquer.
                                            </p>
                                            <p style="color:#D1D5DB; font-size:11px; margin:0;">
                                                © {DateTime.UtcNow.Year} Code Clash. All rights reserved.
                                            </p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>

                    </table>

                    <!-- Bottom spacing -->
                    <p style="color:#9CA3AF; font-size:12px; margin:24px 0 0 0; text-align:center;">
                        You're receiving this email because you created a Code Clash account.
                    </p>

                </td>
            </tr>
        </table>

    </body>
    </html>
    """;

        await SendEmailAsync(user.Email!, htmlMessage, "Confirming Email");
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
                TextBody = "Please confirm your email by visiting the link sent to you."
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

