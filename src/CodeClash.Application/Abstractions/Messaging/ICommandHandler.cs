using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Abstractions.Messaging;
/// <summary>
/// Defines a handler for a command without a response.
/// 
/// Enforces:
/// - Command must implement ICommand
/// - Returns a Result indicating operation success/failure
/// </summary>
public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
}

/// <summary>
/// Defines a handler for a command that returns a response.
/// 
/// Used when a command needs to return data after execution.
/// </summary>
public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
}
