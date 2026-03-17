using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Exceptions;
using FluentValidation;
using MediatR;

namespace CodeClash.Application.Behaviors;
/// <summary>
/// Pipeline behavior responsible for validating commands
/// using FluentValidation before executing the handler.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> validators) // All validators for this request
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // If no validators are registered, continue execution
        if (!validators.Any())
        {
            return await next(cancellationToken);
        }

        // Create validation context for the current request
        var context = new ValidationContext<TRequest>(request);

        // Execute all validators and collect validation failures
        var validationErrors = validators
            .Select(validator => validator.Validate(context)) // Run each validator
            .Where(validationResult => validationResult.Errors.Any()) // Keep only failed results
            .SelectMany(validationResult => validationResult.Errors)  // Flatten all errors
            .Select(validationFailure => new ValidationError(
                validationFailure.PropertyName,
                validationFailure.ErrorMessage)) // Map to custom ValidationError
            .ToList();

        // If any validation errors exist, throw custom exception
        if (validationErrors.Any())
        {
            throw new Exceptions.ValidationException(validationErrors);
        }

        // If validation passes, continue to next behavior or handler
        return await next(cancellationToken);
    }
}
