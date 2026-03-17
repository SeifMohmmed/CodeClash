using CodeClash.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CodeClash.API.Middleware;

/// <summary>
/// Middleware responsible for handling all unhandled exceptions
/// and returning standardized HTTP responses using ProblemDetails.
/// </summary>
public class ExceptionHandlingMiddleware
{
    // Delegate representing the next middleware in the pipeline
    private readonly RequestDelegate _next;

    // Logger used to log exceptions
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Constructor used for dependency injection.
    /// </summary>
    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Main middleware execution method.
    /// It wraps the request pipeline in a try/catch block
    /// to intercept any unhandled exceptions.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Pass the request to the next middleware in the pipeline
            await _next(context);
        }
        catch (Exception exception)
        {
            // Log the exception with its message
            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            // Convert the exception into structured error details
            var exceptionDetails = GetExceptionDetails(exception);

            // Create a standardized API error response using ProblemDetails
            var problemDetails = new ProblemDetails
            {
                Status = exceptionDetails.Status,
                Type = exceptionDetails.Type,
                Title = exceptionDetails.Title,
                Detail = exceptionDetails.Detail,
            };

            // If validation errors exist, attach them to the response
            if (exceptionDetails.Errors is not null)
            {
                problemDetails.Extensions["errors"] = exceptionDetails.Errors;
            }

            // Set the HTTP status code
            context.Response.StatusCode = exceptionDetails.Status;

            // Return the error response as JSON
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }

    /// <summary>
    /// Maps known exceptions to appropriate HTTP responses.
    /// Unknown exceptions are treated as internal server errors.
    /// </summary>
    private static ExceptionDetails GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            // Handles FluentValidation exceptions
            ValidationException validationException => new ExceptionDetails(
                StatusCodes.Status400BadRequest,
                "ValidationFailure",
                "Validation error",
                "One or more validation errors has occurred",
                validationException.Errors),

            // Default case: any unhandled exception
            _ => new ExceptionDetails(
                StatusCodes.Status500InternalServerError,
                "ServerError",
                "Server error",
                "An unexpected error has occurred",
                null)
        };
    }

    /// <summary>
    /// Record used to store structured exception details
    /// before converting them into ProblemDetails.
    /// </summary>
    internal sealed record ExceptionDetails(
        int Status,
        string Type,
        string Title,
        string Detail,
        IEnumerable<object>? Errors);
}
