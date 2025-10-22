using MagnaWms.Contracts.Errors;

namespace MagnaWms.Application.Core.Errors;

public sealed record Error(ErrorCode Code, string Description)
{
    public static readonly Error None = new(ErrorCode.Unknown, string.Empty);

    public bool IsNone => Code == ErrorCode.Unknown;

    // Factory helpers
    public static Error Validation(string message)
        => new(ErrorCode.ValidationFailed, message);

    public static Error NotFound(string entity, object key)
        => new(ErrorCode.NotFound, $"{entity} with key '{key}' was not found.");

    public static Error Conflict(string entity, string reason)
        => new(ErrorCode.Conflict, $"{entity} conflict: {reason}");

    public static Error Unauthorized(string message = "Unauthorized access.")
        => new(ErrorCode.Unauthorized, message);

    public static Error Failure(string message = "An unexpected error occurred.")
        => new(ErrorCode.InternalError, message);
}
