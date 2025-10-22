using MagnaWms.Application.Core.Errors;
using MagnaWms.Contracts.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using ProblemDetailsFactory = Hellang.Middleware.ProblemDetails.ProblemDetailsFactory;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

namespace MagnaWms.Api.Behaviors;

public sealed class MagnaProblemDetailsFactory : ProblemDetailsFactory
{
    private const string TypeBase = "https://magna-wms/errors/";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MagnaProblemDetailsFactory(
        IOptions<ProblemDetailsOptions> options,
        ILogger<ProblemDetailsFactory> logger,
        IHostEnvironment environment,
        IHttpContextAccessor httpContextAccessor) 
        : base(options, logger, environment) 
    => _httpContextAccessor = httpContextAccessor;

    public override ProblemDetails CreateProblemDetails(
        HttpContext context,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        int status = statusCode ?? StatusCodes.Status500InternalServerError;

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title ?? DefaultTitleFor(status),
            Type = type ?? $"{TypeBase}{SlugFor(status)}",
            Detail = detail,
            Instance = instance ?? context.Request.Path
        };

        Enrich(context, problem);
        return problem;
    }

    public override ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext context,
        ModelStateDictionary modelStateDictionary,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        ArgumentNullException.ThrowIfNull(modelStateDictionary);

        int status = statusCode ?? StatusCodes.Status400BadRequest;

        var problem = new ValidationProblemDetails(modelStateDictionary)
        {
            Status = status,
            Title = title ?? "Validation failed",
            Type = type ?? $"{TypeBase}validation-failed",
            Detail = detail,
            Instance = instance ?? context.Request.Path
        };

        Enrich(context, problem);
        return problem;
    }

    public ProblemDetails CreateFromError(Error error)
    {
        HttpContext? context = _httpContextAccessor.HttpContext;
        (int status, string title) = MapStatus(error.Code);

        ProblemDetails problem = CreateProblemDetails(
            context ?? new DefaultHttpContext(),
            statusCode: status,
            title: title,
            type: $"{TypeBase}{SlugFor(status)}",
            detail: error.Description
        );

        problem.Extensions["code"] = (int)error.Code;

        if (error.Code == ErrorCode.ValidationFailed &&
            context != null &&
            context.Items.TryGetValue("validationErrors", out object? errors) &&
            errors is IDictionary<string, string[]> validationDict)
        {
            problem.Extensions["errors"] = validationDict;
        }

        return problem;
    }

    private static (int status, string title) MapStatus(ErrorCode code) => code switch
    {
        ErrorCode.ValidationFailed => (400, "Validation Failed"),
        ErrorCode.BadRequest => (400, "Bad Request"),
        ErrorCode.Conflict => (409, "Conflict"),
        ErrorCode.Forbidden => (403, "Forbidden"),
        ErrorCode.Unauthorized => (401, "Unauthorized"),
        ErrorCode.NotFound => (404, "Resource Not Found"),
        ErrorCode.ConcurrencyConflict => (409, "Concurrency Conflict"),
        ErrorCode.DatabaseError => (500, "Database Error"),
        ErrorCode.DatabaseUnavailable => (503, "Database Unavailable"),
        ErrorCode.InternalError => (500, "Internal Server Error"),
        _ => (500, "Unknown Error")
    };

    private static void Enrich(HttpContext context, ProblemDetails problem)
    {
        // Always include traceId and a correlationId
        problem.Extensions["traceId"] = context.TraceIdentifier;

        string? cid = null;

        // Prefer item stored by CorrelationIdMiddleware
        if (context.Items.TryGetValue("X-Correlation-ID", out object? itemCid) && itemCid is string itemVal)
        {
            cid = itemVal;
        }
        else if (context.Request.Headers.TryGetValue("X-Correlation-ID", out StringValues headerCid) && !StringValues.IsNullOrEmpty(headerCid))
        {
            cid = headerCid.ToString();
        }

        cid ??= Guid.NewGuid().ToString();
        problem.Extensions["correlationId"] = cid.ToString();
    }

    private static string DefaultTitleFor(int status) => status switch
    {
        400 => "Bad Request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        405 => "Method Not Allowed",
        409 => "Conflict",
        422 => "Unprocessable Entity",
        500 => "Internal Server Error",
        _ => "Internal Server Error"
    };

    private static string SlugFor(int status) => status switch
    {
        400 => "bad-request",
        401 => "unauthorized",
        403 => "forbidden",
        404 => "not-found",
        405 => "method-not-allowed",
        409 => "conflict",
        422 => "unprocessable-entity",
        500 => "internal-error",
        _ => $"status-{status}"
    };
}
