using System.Reflection;
using EvrenDev.Infrastructure.Auth;
using EvrenDev.Infrastructure.BackgroundJobs;
using EvrenDev.Infrastructure.Caching;
using EvrenDev.Infrastructure.Common;
using EvrenDev.Infrastructure.Cors;
using EvrenDev.Infrastructure.FileStorage;
using EvrenDev.Infrastructure.Localization;
using EvrenDev.Infrastructure.Mailing;
using EvrenDev.Infrastructure.Mapping;
using EvrenDev.Infrastructure.Middleware;
using EvrenDev.Infrastructure.Multitenancy;
using EvrenDev.Infrastructure.Notifications;
using EvrenDev.Infrastructure.OpenApi;
using EvrenDev.Infrastructure.Persistence;
using EvrenDev.Infrastructure.Persistence.Initialization;
using EvrenDev.Infrastructure.SecurityHeaders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvrenDev.Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        MapsterSettings.Configure();
        return services
            .AddApiVersioning()
            .AddAuth(config)
            .AddBackgroundJobs(config)
            .AddCaching(config)
            .AddCorsPolicy(config)
            .AddExceptionMiddleware()
            .AddHealthCheck()
            .AddLocalization(config)
            .AddMailing(config)
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
            .AddMultitenancy(config)
            .AddNotifications(config)
            .AddOpenApiDocumentation(config)
            .AddPersistence(config)
            .AddRequestLogging(config)
            .AddRouting(options => options.LowercaseUrls = true)
            .AddServices();
    }

    private static IServiceCollection AddApiVersioning(this IServiceCollection services)
    {
        return services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });
    }

    private static IServiceCollection AddHealthCheck(this IServiceCollection services)
    {
        return services.AddHealthChecks().AddCheck<TenantHealthCheck>("Tenant").Services;
    }

    public static async Task InitializeDatabasesAsync(this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        // Create a new scope to retrieve scoped services
        using var scope = services.CreateScope();

        await scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>()
            .InitializeDatabasesAsync(cancellationToken);
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder, IConfiguration config)
    {
        return builder
            .UseLocalization(config)
            .UseStaticFiles()
            .UseSecurityHeaders(config)
            .UseFileStorage()
            .UseExceptionMiddleware()
            .UseRouting()
            .UseCorsPolicy()
            .UseAuthentication()
            .UseCurrentUser()
            .UseMultiTenancy()
            .UseAuthorization()
            .UseRequestLogging(config)
            .UseHangfireDashboard(config)
            .UseOpenApiDocumentation(config);
    }

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapControllers().RequireAuthorization();
        builder.MapHealthCheck();
        builder.MapNotifications();
        return builder;
    }

    private static IEndpointConventionBuilder MapHealthCheck(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapHealthChecks("/api/health").RequireAuthorization();
    }
}
