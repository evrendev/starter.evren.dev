using System.Reflection;
using Ardalis.GuardClauses;
using EvrenDev.Infrastructure.Audit.Data;
using EvrenDev.Infrastructure.Catalog.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        var auditConnectionString = configuration.GetConnectionString("AuditConnection");
        Guard.Against.Null(auditConnectionString, message: "Audit Connection string 'AuditConnection' not found.");
        services.AddDbContext<AuditLogDbContext>(options => options.UseSqlServer(auditConnectionString));

        services.AddSingleton(TimeProvider.System);

        services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
