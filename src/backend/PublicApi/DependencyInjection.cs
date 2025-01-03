using EvrenDev.Infrastructure.Audit.Data;
using EvrenDev.Infrastructure.Catalog.Data;
using EvrenDev.Infrastructure.Identity.Data;
using EvrenDev.Infrastructure.Tenant.Data;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>()
            .AddDbContextCheck<IdentityDbContext>()
            .AddDbContextCheck<AuditLogDbContext>()
            .AddDbContextCheck<TenantDbContext>();

        return services;
    }
}
