using MagnaWms.Application.Core.Errors;

namespace MagnaWms.Application.Core.Results;

/// <summary>
/// Represents a typed result that can be either Success or Failure.
/// </summary>
public sealed class Result<T>
{
    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        Error = Error.None;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
        Value = default!;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public T Value { get; }
    public Error Error { get; }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);

    public void Deconstruct(out bool isSuccess, out T value, out Error error)
    {
        isSuccess = IsSuccess;
        value = Value;
        error = Error;
    }
}
