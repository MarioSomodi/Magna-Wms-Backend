namespace MagnaWms.Domain.Core.Exceptions;

/// <summary>
/// Represents a business rule violation inside the domain layer.
/// Handlers catch this and translate to ProblemDetails errors.
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public DomainException()
    {
    }

    public DomainException(string message) : base(message)
    {
    }
}
