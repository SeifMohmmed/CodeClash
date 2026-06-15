using CodeClash.Application.Abstractions.Email;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Emails.SendEmails;

internal sealed class SendEmailCommandHandler(
    IEmailService emailService) : ICommandHandler<SendEmailCommand, Unit>
{
    public async Task<Result<Unit>> Handle(
        SendEmailCommand request,
        CancellationToken cancellationToken)
    {
        var sent = await emailService.SendEmailAsync(
            request.Email,
            request.Message,
            subject: request.Subject);

        return sent
            ? Result.Success(Unit.Value)
            : Result.Failure<Unit>(EmailErrors.SendFailed);

    }
}
