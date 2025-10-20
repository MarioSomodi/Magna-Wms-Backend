using Serilog;
using Serilog.Context;

namespace MagnaWms.Api.Middleware;

public static class LoggingContextMiddleware
{
    /// <summary>
    /// Push CorrelationId and UserId into Serilog's LogContext so all logs have them.
    /// Must run before UseSerilogRequestLogging().
    /// </summary>
    public static WebApplication UseRequestContextLogging(this WebApplication app)
    {
        app.Use(async (ctx, next) =>
        {
            // CorrelationId set by UseCorrelationId() → header or generated
            string? cid = ctx.Items.TryGetValue("X-Correlation-ID", out object? v) ? v?.ToString()
                    : (ctx.Request.Headers.TryGetValue("X-Correlation-ID", out Microsoft.Extensions.Primitives.StringValues hv) ? hv.ToString() : null);

            // UserId (prefer stable subject/ID claim; fallback to NameIdentifier or Name)
            string? userId = ctx.User?.FindFirst("sub")?.Value
                          ?? ctx.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                          ?? (ctx.User?.Identity?.IsAuthenticated == true ? ctx.User.Identity!.Name : null);

            using (LogContext.PushProperty("CorrelationId", cid ?? "n/a"))
            using (LogContext.PushProperty("UserId", userId ?? "anonymous"))
            {
                await next();
            }
        });

        app.UseSerilogRequestLogging(opts =>
            // Include request method/host/path, status code, elapsed ms by default
            opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms");

        return app;
    }
}
