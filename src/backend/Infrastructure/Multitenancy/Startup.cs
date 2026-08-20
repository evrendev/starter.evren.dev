using EvrenDev.Application.Multitenancy.Interfaces;
using EvrenDev.Infrastructure.Persistence;
using EvrenDev.Shared.Authorization;
using EvrenDev.Shared.Multitenancy;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.AspNetCore;
using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;
using Finbuckle.MultiTenant.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TenantInfo = EvrenDev.Domain.Multitenancy.TenantInfo;

namespace EvrenDev.Infrastructure.Multitenancy;

internal static class Startup
{
    internal static IServiceCollection AddMultitenancy(this IServiceCollection services, IConfiguration config)
    {
        // TODO: We should probably add specific dbprovider/connectionstring setting for the tenantDb with a fallback to the main databasesettings
        var databaseSettings = config.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        var rootConnectionString = databaseSettings!.ConnectionString;
        if (string.IsNullOrEmpty(rootConnectionString))
            throw new InvalidOperationException("DB ConnectionString is not configured.");
        var dbProvider = databaseSettings.DbProvider;
        if (string.IsNullOrEmpty(dbProvider))
            throw new InvalidOperationException("DB Provider is not configured.");

        return services
            .AddDbContext<TenantDbContext>(m => m.UseDatabase(dbProvider, rootConnectionString))
            .AddMultiTenant<TenantInfo>()
            .WithClaimStrategy(ApiClaims.Tenant)
            .WithHeaderStrategy(MultitenancyConstants.TenantIdName)
            .WithQueryStringStrategy(MultitenancyConstants.TenantIdName)
            .WithEFCoreStore<TenantDbContext, TenantInfo>()
            .Services
            .AddScoped<ITenantService, TenantService>();
    }

    internal static IApplicationBuilder UseMultiTenancy(this IApplicationBuilder app)
    {
        return app.UseMultiTenant();
    }

    private static MultiTenantBuilder<TenantInfo> WithQueryStringStrategy(
        this MultiTenantBuilder<TenantInfo> builder, string queryStringKey)
    {
        return builder.WithDelegateStrategy(context =>
        {
            if (context is not HttpContext httpContext)
                return Task.FromResult((string?)null);

            httpContext.Request.Query.TryGetValue(queryStringKey, out var tenantIdParam);

            return Task.FromResult((string?)tenantIdParam.ToString());
        });
    }
}
