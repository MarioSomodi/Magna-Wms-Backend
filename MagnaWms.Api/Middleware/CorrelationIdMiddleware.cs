using Microsoft.Extensions.Primitives;

namespace MagnaWms.Api.Middleware;

public static class CorrelationIdExtensions
{
    private const string HeaderName = "X-Correlation-ID";

    /// <summary>
    /// Ensures every request and response carries an <c>X-Correlation-ID</c> header.
    /// If the client does not provide one, a new GUID is generated.
    /// </summary>
    /// <remarks>
    /// This middleware runs early in the pipeline to make correlation IDs
    /// available to all downstream components (logging, tracing, ProblemDetails).
    /// </remarks>
    public static WebApplication UseCorrelationId(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            // Read or generate correlation ID
            if (!context.Request.Headers.TryGetValue(HeaderName, out StringValues cid) ||
                string.IsNullOrWhiteSpace(cid))
            {
                cid = Guid.NewGuid().ToString();
            }

            // Store it in Items for easy downstream retrieval
            context.Items[HeaderName] = cid.ToString();

            // Echo back in response headers
            context.Response.Headers[HeaderName] = cid.ToString();

            await next().ConfigureAwait(false);
        });

        return app;
    }
}
