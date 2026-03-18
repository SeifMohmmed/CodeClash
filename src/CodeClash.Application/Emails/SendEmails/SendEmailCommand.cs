using CodeClash.Application.Abstractions.Messaging;
using MediatR;

namespace CodeClash.Application.Emails.SendEmails;
public record class SendEmailCommand(
    string Email,
    string Message) : ICommand<Unit>;

