using MagnaWms.Api.Extensions;
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
            .AddApiVersioningWithExplorer()
            .AddSwaggerDocumentation()
            .AddProblemDetailsSupport(builder.Environment)
            .AddOpenTelemetryStubs(serviceName: "MagnaWms.Api")

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
