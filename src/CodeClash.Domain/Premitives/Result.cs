using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using CodeClash.Domain.Abstractions;

namespace CodeClash.Domain.Premitives;
/// <summary>
/// Represents the result of an operation (success or failure).
/// </summary>
public class Result
{
    protected internal Result(bool isSuccess, Error? error, string? message = null)
    {
        // Success must have Error.None
        if (isSuccess && error is not null)
        {
            throw new InvalidOperationException();
        }

        // Failure must have a real error
        if (!isSuccess && error is null)
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
    [JsonIgnore]
    public bool IsFailure => !IsSuccess;

    /// <summary>The error associated with a failure.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Error? Error { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; }

    /// <summary>Create a successful result.</summary>
    public static Result Success(string message = "")
        => new(true, null, message);

    /// <summary>Create a failed result.</summary>
    public static Result Failure(Error error)
        => new(false, error, null);

    /// <summary>Create a successful result with a value.</summary>
    public static Result<TValue> Success<TValue>(TValue value, string message = "")
        => new(value, true, null, message);

    /// <summary>Create a failed result with a value type.</summary>
    public static Result<TValue> Failure<TValue>(Error error)
        => new(default, false, error, null);

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
        Error? error,
        string? message = null)
        : base(isSuccess, error, message)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value if success; throws if failure.
    /// </summary>
    [NotNull]
    [JsonIgnore] // prevents serializer from calling this on failure results
    public TValue Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TValue? Data => IsSuccess ? _value : default;

    /// <summary>
    /// Allows implicit conversion from TValue to Result&lt;TValue&gt;.
    /// </summary>
    public static implicit operator Result<TValue>(TValue? value) => Create(value);
}
