using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Extensions.Logging;
using TenantInfo = EvrenDev.Domain.Multitenancy.TenantInfo;

namespace EvrenDev.Infrastructure.Persistence.Initialization;

// ITenantInfo is no longer directly DI-resolvable as of Finbuckle v10 (Task S3).
internal class ApplicationDbInitializer(
    ApplicationDbContext dbContext,
    IMultiTenantContextAccessor<TenantInfo> multiTenantContextAccessor,
    ApplicationDbSeeder dbSeeder,
    ILogger<ApplicationDbInitializer> logger)
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var currentTenantId = multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id;

        if (dbContext.Database.GetMigrations().Any())
        {
            if ((await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
            {
                logger.LogInformation("Applying Migrations for '{tenantId}' tenant.", currentTenantId);
                await dbContext.Database.MigrateAsync(cancellationToken);
            }

            if (await dbContext.Database.CanConnectAsync(cancellationToken))
            {
                logger.LogInformation("Connection to {tenantId}'s Database Succeeded.", currentTenantId);

                await dbSeeder.SeedDatabaseAsync(dbContext, cancellationToken);
            }
        }
    }
}
