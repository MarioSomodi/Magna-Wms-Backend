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

    public MagnaProblemDetailsFactory(IOptions<ProblemDetailsOptions> options, ILogger<ProblemDetailsFactory> logger, IHostEnvironment environment) : base(options, logger, environment)
    {
    }

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

    private static void Enrich(HttpContext context, ProblemDetails problem)
    {
        // Always include traceId and a correlationId
        problem.Extensions["traceId"] = context.TraceIdentifier;

        if (!context.Request.Headers.TryGetValue("X-Correlation-ID", out StringValues cid) || string.IsNullOrWhiteSpace(cid))
        {
            cid = Guid.NewGuid().ToString();
        }
        problem.Extensions["correlationId"] = cid.ToString();

        // Optional: bubble up a domain/application error code if a handler set one
        if (context.Items.TryGetValue("errorCode", out object? code) && code is string s && !string.IsNullOrWhiteSpace(s))
        {
            problem.Extensions["errorCode"] = s;
            // Make the RFC7807 type stable & shareable:
            problem.Type = $"{TypeBase}{s}";
        }
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
        _ => "An error occurred"
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
