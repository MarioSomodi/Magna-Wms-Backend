using Hellang.Middleware.ProblemDetails;
using MagnaWms.Api.Configuration;
using MagnaWms.Api.Middleware;
using MagnaWms.Application.Core;

namespace MagnaWms.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.AddSerilogLogging();
        
        builder.Services
            .AddControllers();
        
        builder.Services
            .AddHttpContextAccessor()
            .AddApiVersioningWithExplorer()
            .AddSwaggerDocumentation()
            .AddProblemDetailsSupport(builder.Environment)
            .AddOpenTelemetryStubs(serviceName: "MagnaWms.Api")
            .AddPersistence(builder.Configuration);

        WebApplication app = builder.Build();

        app.UseCorrelationId();
        // pushes CorrelationId/UserId into LogContext
        app.UseRequestContextLogging();
        app.UseProblemDetails();
        app.UseSwaggerUIWithVersions();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
