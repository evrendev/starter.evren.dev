using System.Reflection;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Entities.Tenant;
using Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore;

namespace EvrenDev.Infrastructure.Tenant.Data;

public class TenantDbContext : EFCoreStoreDbContext<AppTenantInfo>, ITenantDbContext
{
    public TenantDbContext(DbContextOptions<TenantDbContext> options)
        : base(options)
    {
    }
    public DbSet<AppTenantInfo> Tenants => Set<AppTenantInfo>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply global query filters for multi-tenancy and soft delete
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var entityClrType = entityType.ClrType;

            // Apply  filter
            if (typeof(BaseAuditableEntity).IsAssignableFrom(entityClrType))
            {
                var filter = typeof(TenantDbContext)
                    .GetMethod(nameof(ApplyFilter), BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.MakeGenericMethod(entityClrType);

                filter?.Invoke(this, [builder]);
            }
        }

        builder.Entity<AppTenantInfo>().ToTable("Tenants");
        builder.HasDefaultSchema("Tenant");
    }

    private void ApplyFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : BaseAuditableEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.Deleted == false);
    }
}
