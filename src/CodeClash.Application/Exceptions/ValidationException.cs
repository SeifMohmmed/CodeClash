namespace CodeClash.Application.Exceptions;
/// <summary>
/// Custom exception thrown when command validation fails.
/// Contains all validation errors.
/// </summary>
public sealed class ValidationException(
    IEnumerable<ValidationError> errors) : Exception
{
    public IEnumerable<ValidationError> Errors { get; } = errors;  // Collection of validation errors.
}
