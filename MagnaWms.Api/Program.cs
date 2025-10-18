using MagnaWms.Api.Extensions;
using MagnaWms.Application;
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
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AssemblyMarker>())
            .AddApiVersioningWithExplorer()
            .AddSwaggerDocumentation()
            .AddProblemDetailsSupport(builder.Environment)
            .AddOpenTelemetryStubs(serviceName: "MagnaWms.Api")
            .AddPersistence(builder.Configuration);

        WebApplication app = builder.Build();

        app.UseCorrelationId();
        // pushes CorrelationId/UserId into LogContext
        app.UseRequestContextLogging();
        app.UseProblemDetailsSupport();
        app.UseSwaggerUIWithVersions();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
