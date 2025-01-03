using System.Reflection;
using Ardalis.GuardClauses;
using Audit.EntityFramework;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Infrastructure.Audit.Data;
using EvrenDev.Infrastructure.Catalog.Interceptors;
using EvrenDev.Infrastructure.Catalog.Data;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        var catalogConnectionString = configuration.GetConnectionString("CatalogConnection");
        Guard.Against.Null(catalogConnectionString, message: "Catalog Connection string 'CatalogConnection' not found.");
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(new AuditSaveChangesInterceptor());
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseSqlServer(catalogConnectionString);
        });

        var auditConnectionString = configuration.GetConnectionString("AuditConnection");
        Guard.Against.Null(auditConnectionString, message: "Audit Connection string 'AuditConnection' not found.");
        services.AddDbContext<AuditLogDbContext>(options => options.UseSqlServer(auditConnectionString));


        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddSingleton(TimeProvider.System);

        services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
