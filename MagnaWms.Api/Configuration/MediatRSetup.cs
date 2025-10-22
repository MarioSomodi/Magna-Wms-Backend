using MagnaWms.Application.Core;
using MagnaWms.Application.Core.Behaviors;
using MediatR;

namespace MagnaWms.Api.Configuration;

public static class MediatRSetup
{
    public static IServiceCollection AddMediatRAndBehaviors(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AssemblyMarker>())
                        .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
                        .AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
        return services;
    }
}
