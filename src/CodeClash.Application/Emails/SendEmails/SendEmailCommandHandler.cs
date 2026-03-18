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
        var result =
            await emailService.SendEmailAsync(request.Email, request.Message, null);

        if (result)
        {
            return Result.Success<Unit>(Unit.Value, "Email Send Successfully");
        }

        return Result.Failure<Unit>(EmailErrors.SendFailed);
    }
}
