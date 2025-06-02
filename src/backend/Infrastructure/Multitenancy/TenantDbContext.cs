using EvrenDev.Infrastructure.Persistence.Configuration;
using Finbuckle.MultiTenant.Stores;

namespace EvrenDev.Infrastructure.Multitenancy;

public class TenantDbContext(DbContextOptions<TenantDbContext> options) : EFCoreStoreDbContext<TenantInfo>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TenantInfo>().ToTable("Tenants", SchemaNames.MultiTenancy);
    }
}
