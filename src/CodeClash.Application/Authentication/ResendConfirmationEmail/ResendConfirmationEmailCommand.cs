using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Authentication.ResendConfirmationEmail;

public record ResendConfirmationEmailCommand(string Email)
    : ICommand<string>;
