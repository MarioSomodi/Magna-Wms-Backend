using Serilog;

namespace MagnaWms.Api.Configuration;

public static class LoggingSetup
{
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Host.UseSerilog();
        return builder;
    }
}
