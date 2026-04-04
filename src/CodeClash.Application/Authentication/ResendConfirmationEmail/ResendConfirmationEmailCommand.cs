using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Authentication.ResendConfirmationEmail;
public record ResendConfirmationEmailCommand(string Email)
    : IRequest<Result<string>>;
