using System.Diagnostics.CodeAnalysis;
using CodeClash.Domain.Abstractions;

namespace CodeClash.Domain.Premitives;
/// <summary>
/// Represents the result of an operation (success or failure).
/// </summary>
public class Result
{
    protected internal Result(bool isSuccess, Error error, string message = "")
    {
        // Success must have Error.None
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException();
        }

        // Failure must have a real error
        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Error = error;
        Message = message;
    }

    /// <summary>Indicates whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Indicates whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>The error associated with a failure.</summary>
    public Error Error { get; }

    public string Message { get; }

    /// <summary>Create a successful result.</summary>
    public static Result Success(string message = "")
        => new(true, Error.None, message);

    /// <summary>Create a failed result.</summary>
    public static Result Failure(Error error)
        => new(false, error, error.Message);

    /// <summary>Create a successful result with a value.</summary>
    public static Result<TValue> Success<TValue>(TValue value, string message = "")
        => new(value, true, Error.None, message);

    /// <summary>Create a failed result with a value type.</summary>
    public static Result<TValue> Failure<TValue>(Error error)
        => new(default, false, error, error.Message);

    /// <summary>Create a result from a nullable value.</summary>
    public static Result<TValue> Create<TValue>(TValue? value) =>
        value is null ? Failure<TValue>(Error.NullValue) : Success(value);
}

/// <summary>
/// Represents the result of an operation that returns a value.
/// </summary>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(
        TValue? value,
        bool isSuccess,
        Error error,
        string message = "")
        : base(isSuccess, error, message)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value if success; throws if failure.
    /// </summary>
    [NotNull]
    public TValue Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    /// <summary>
    /// Allows implicit conversion from TValue to Result&lt;TValue&gt;.
    /// </summary>
    public static implicit operator Result<TValue>(TValue? value) => Create(value);
}
