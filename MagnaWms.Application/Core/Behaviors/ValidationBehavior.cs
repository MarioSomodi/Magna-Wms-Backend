using FluentValidation;
using MediatR;
using MagnaWms.Application.Core.Errors;
using MagnaWms.Application.Core.Results;
using MagnaWms.Contracts.Errors;
using FluentValidation.Results;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace MagnaWms.Application.Core.Behaviors;

/// <summary>
/// Pipeline behavior that runs FluentValidation validators before the handler.
/// Converts validation errors into a standardized Result failure.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators, IHttpContextAccessor httpContextAccessor) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);
        ValidationResult[] results = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = results
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next(cancellationToken);
        }

        // Group by property name for structured field-level errors
        var fieldErrors = failures
            .GroupBy(f => f.PropertyName, StringComparer.Ordinal)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray(), StringComparer.Ordinal);

        string description = "One or more validation errors occurred.";
        HttpContext? httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is not null)
        {
            httpContext.Items["validationErrors"] = fieldErrors;
        }
        var error = new Error(ErrorCode.ValidationFailed, description);

        // If TResponse is Result<>, construct failure
        Type responseType = typeof(TResponse);
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type genericArg = responseType.GetGenericArguments()[0];
            MethodInfo method = typeof(Result<>)
                .MakeGenericType(genericArg)
                .GetMethod(nameof(Result<object>.Failure))!;

            return (TResponse)method.Invoke(null, new object[] { error })!;
        }
        return default!;
    }
}
