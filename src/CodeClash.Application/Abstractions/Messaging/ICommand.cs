using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Abstractions.Messaging;
/// <summary>
/// Marker interface representing a command without a response.
/// A command represents an intention to change the state of the system.
/// 
/// In CQRS:
/// - Commands mutate state
/// - They return a Result indicating success or failure
/// </summary>
public interface ICommand : IRequest<Result>, IBaseCommand
{
}

/// <summary>
/// Represents a command that returns a response of type <typeparamref name="TResponse"/>.
/// 
/// Used when a state-changing operation needs to return data
/// (e.g., returning created entity ID).
/// </summary>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
{
}

/// <summary>
/// Base marker interface for all commands.
/// 
/// Allows common constraints, behaviors, or pipeline behaviors
/// to target all commands generically.
/// </summary>
public interface IBaseCommand
{
}
