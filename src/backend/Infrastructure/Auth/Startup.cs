using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Infrastructure.Auth.Jwt;
using EvrenDev.Infrastructure.Auth.Permissions;
using EvrenDev.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvrenDev.Infrastructure.Auth;

internal static class Startup
{
    internal static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration config)
    {
        services.AddCurrentUser()
            .AddPermissions()
            .AddIdentity()
            .AddTotpService();

        services.Configure<SecuritySettings>(config.GetSection(nameof(SecuritySettings)));

        return services.AddJwtAuth(config);
    }

    internal static IApplicationBuilder UseCurrentUser(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CurrentUserMiddleware>();
    }

    private static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        return services
            .AddScoped<CurrentUserMiddleware>()
            .AddScoped<ICurrentUser, CurrentUser>()
            .AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());
    }

    private static IServiceCollection AddPermissions(this IServiceCollection services)
    {
        return services
            .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
    }
}
