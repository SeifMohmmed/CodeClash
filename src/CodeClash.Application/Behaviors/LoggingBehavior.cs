using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Premitives;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace CodeClash.Application.Behaviors;
/// <summary>
/// MediatR pipeline behavior responsible for logging the lifecycle
/// of command execution.
///
/// It logs:
/// - When a command starts executing
/// - Whether the command succeeds or fails
/// - Any exceptions thrown during execution
///
/// This behavior helps provide structured logging for application commands.
/// </summary>
public class LoggingBehavior<TRequest, TResponse>
    (ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : Result
{
    /// <summary>
    /// Handles the execution of the request through the pipeline.
    /// </summary>
    /// <param name="request">The incoming command request.</param>
    /// <param name="next">Delegate representing the next step in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result returned by the request handler.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Get the name of the command being executed
        var name = request.GetType().Name;

        try
        {
            // Log command execution start
            logger.LogInformation("Executing request {Request}", name);

            // Execute next behavior or handler
            var result = await next(cancellationToken);

            if (result.IsSuccess)
            {
                // Log after successful execution
                logger.LogInformation("Request {Request} processed successfully", name);
            }
            else
            {
                // Attach error details to structured logs
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Request {Request} processed with error", name);
                }
            }


            return result;
        }

        catch (Exception ex)
        {
            // Log error if execution fails
            logger.LogError(ex, "Request {Request} processing failed", name);

            throw; // Rethrow exception to preserve pipeline behavior
        }
    }
}
