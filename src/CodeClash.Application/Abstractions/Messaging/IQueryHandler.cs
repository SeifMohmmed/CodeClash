using CodeClash.Domain.Premitives;
using MediatR;

namespace CodeClash.Application.Abstractions.Messaging;
/// <summary>
/// Defines a handler for a query.
/// 
/// Enforces:
/// - Query must implement IQuery<TResponse>
/// - Returns a Result<TResponse>
/// - Should contain read-only logic
/// </summary>
public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{

}
