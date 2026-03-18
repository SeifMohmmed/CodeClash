using Serilog.Context;

namespace CodeClash.API.Middleware;

/// <summary>
/// Middleware responsible for attaching a Correlation ID to every log entry.
///
/// The correlation ID allows tracking a single request across multiple services
/// in a distributed or microservices architecture.
///
/// The middleware checks if the incoming request already contains a correlation ID
/// in the request header. If it exists, it uses that value; otherwise, it generates
/// one using ASP.NET Core's built-in TraceIdentifier.
/// </summary>
public class RequestContextLoggingMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeaderName = "X-Correlation-Id"; // Header name used for passing the correlation ID between services.

    /// <summary>
    /// Executes the middleware logic.
    /// Adds the correlation ID to the Serilog logging context.
    /// </summary>
    public Task Invoke(HttpContext httpContext)
    {
        // Push the CorrelationId property into the Serilog log context
        // so it automatically appears in all logs within this request.
        using (LogContext.PushProperty("CorrelationId", GetCorrelationId(httpContext)))
        {
            return next(httpContext);
        }
    }

    /// <summary>
    /// Retrieves the correlation ID from the request headers.
    /// If it does not exist, the ASP.NET Core TraceIdentifier is used instead.
    /// </summary>
    private static string GetCorrelationId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId);

        // Use provided correlation ID or fallback to TraceIdentifier
        return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
    }
}
