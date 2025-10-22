using System.Reflection;
using MediatR;
using Microsoft.Extensions.Logging;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Errors;

namespace MagnaWms.Application.Core.Behaviors;

/// <summary>
/// Global MediatR pipeline behavior that catches all unhandled exceptions
/// and converts them into standardized <see cref="Result{T}"/> failures.
/// Ensures no exception escapes the Application layer.
/// </summary>
public sealed class ExceptionHandlingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;

    public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Let cancellations bubble up naturally.
            _logger.LogWarning("Request {RequestType} was canceled.", typeof(TRequest).Name);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception occurred while processing request {RequestType}.",
                typeof(TRequest).Name);

            // Check if TResponse is a Result<>
            Type responseType = typeof(TResponse);
            if (responseType.IsGenericType &&
                responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                Type genericArg = responseType.GetGenericArguments()[0];
                MethodInfo failureMethod = typeof(Result<>)
                    .MakeGenericType(genericArg)
                    .GetMethod(nameof(Result<object>.Failure), BindingFlags.Public | BindingFlags.Static)!;

                var error = new Error(ErrorCode.InternalError, ex.Message);
                return (TResponse)failureMethod.Invoke(null, new object[] { error })!;
            }

            // Not a Result<T> response → rethrow for upper middleware
            throw;
        }
    }
}
