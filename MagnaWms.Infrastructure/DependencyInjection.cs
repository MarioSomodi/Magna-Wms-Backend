using System.Text;
using MagnaWms.Application.Core.Abstractions.Authentication;
using MagnaWms.Application.Core.Options;
using MagnaWms.Infrastructure.Authentication;
using MagnaWms.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MagnaWms.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddAuthenticationInternal(configuration)
            .AddAuthorizationInternal();

        // Authentication services
        services.AddScoped<ITokenProvider, JwtTokenProvider>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        //Autorization
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0051:Method is too long", Justification = "Needed")]
    private static IServiceCollection AddAuthenticationInternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        JwtOptions jwtOptions = services.BuildServiceProvider().GetService<IOptions<JwtOptions>>()!.Value;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret ?? throw new InvalidOperationException("Jwt:Secret is not configured.")));

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        if (ctx.Request.Cookies.TryGetValue("auth", out string? jwt) &&
                            !string.IsNullOrWhiteSpace(jwt))
                        {
                            ctx.Token = jwt;
                        }

                        return Task.CompletedTask;
                    },
                    OnChallenge = async ctx =>
                    {
                        ctx.HandleResponse();

                        var problem = new ProblemDetails
                        {
                            Status = StatusCodes.Status401Unauthorized,
                            Title = "Unauthorized",
                            Detail = "Authentication is required.",
                            Type = "https://magna-wms/errors/unauthorized",
                            Instance = ctx.Request.Path
                        };

                        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        ctx.Response.ContentType = "application/problem+json";
                        await ctx.Response.WriteAsJsonAsync(problem, cancellationToken: ctx.HttpContext.RequestAborted);
                    },
                    OnForbidden = async ctx =>
                    {
                        var problem = new ProblemDetails
                        {
                            Status = StatusCodes.Status403Forbidden,
                            Title = "Forbidden",
                            Detail = "You do not have permission to access this resource.",
                            Type = "https://magna-wms/errors/forbidden",
                            Instance = ctx.Request.Path
                        };

                        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                        ctx.Response.ContentType = "application/problem+json";
                        await ctx.Response.WriteAsJsonAsync(problem, cancellationToken: ctx.HttpContext.RequestAborted);
                    }
                };
            });

        return services;
    }

    private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services) =>
        // RBAC + ABAC policies will be added here later
        services;
}
