using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using MagnaWms.Api.Configuration;
using MagnaWms.Api.Middleware;
using MagnaWms.Application.Core;
using MagnaWms.Application.Core.Mapping;
using MagnaWms.Infrastructure;

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
            .AddCorsSupport(builder.Configuration)
            .AddHttpContextAccessor()
            .AddValidatorsFromAssemblyContaining<AssemblyMarker>()
            .AddMediatRAndBehaviors()
            .AddApiVersioningWithExplorer()
            .AddSwaggerDocumentation()
            .AddProblemDetailsSupport(builder.Environment)
            .AddOpenTelemetryStubs(serviceName: "MagnaWms.Api")
            .AddPersistence(builder.Configuration)
            .AddMapping()
            .AddInfrastructure(builder.Configuration);

        WebApplication app = builder.Build();
        app.UseCorsSupport();
        app.UseCorrelationId();
        // pushes CorrelationId/UserId into LogContext
        app.UseRequestContextLogging();
        app.UseProblemDetails();
        app.UseSwaggerUIWithVersions();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
