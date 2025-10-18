namespace MagnaWms.Contracts.Errors;

public enum ErrorCode
{
    Unknown = 0,

    // 400s
    ValidationFailed = 100,
    BadRequest = 101,
    Conflict = 102,
    Forbidden = 103,

    // 401/403
    Unauthorized = 200,

    // 404
    NotFound = 300,

    // 409
    ConcurrencyConflict = 400,

    // 500s
    DatabaseError = 900,
    DatabaseUnavailable = 901,
    InternalError = 999
}
