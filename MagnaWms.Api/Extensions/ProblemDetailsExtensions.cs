using Hellang.Middleware.ProblemDetails;
using MagnaWms.Api.Behaviors;
using Microsoft.AspNetCore.Mvc;
using ProblemDetailsFactory = Hellang.Middleware.ProblemDetails.ProblemDetailsFactory;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

namespace MagnaWms.Api.Extensions;

public static class ProblemDetailsExtensions
{
    public static IServiceCollection AddProblemDetailsSupport(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddProblemDetails(options =>
        {
            // Include exception details only in Development
            options.IncludeExceptionDetails = (ctx, ex) => env.IsDevelopment();

            // Map known exception types to status codes (you can add your own)
            options.MapToStatusCode<ArgumentException>(StatusCodes.Status400BadRequest);
            options.MapToStatusCode<UnauthorizedAccessException>(StatusCodes.Status401Unauthorized);
            options.MapToStatusCode<KeyNotFoundException>(StatusCodes.Status404NotFound);
            options.MapToStatusCode<InvalidOperationException>(StatusCodes.Status409Conflict);
            options.MapToStatusCode<NotSupportedException>(StatusCodes.Status405MethodNotAllowed);
            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
            options.ValidationProblemStatusCode = StatusCodes.Status422UnprocessableEntity;
        });

        // Replace the default factory
        services.AddSingleton<ProblemDetailsFactory, MagnaProblemDetailsFactory>();

        return services;
    }

    public static WebApplication UseProblemDetailsSupport(this WebApplication app)
    {
        // ProblemDetails must be early in the pipeline to catch exceptions
        app.UseProblemDetails();
        return app;
    }
}
