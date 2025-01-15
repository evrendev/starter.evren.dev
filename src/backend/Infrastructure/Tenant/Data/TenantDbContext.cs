using System.Reflection;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Entities.Tenant;

namespace EvrenDev.Infrastructure.Tenant.Data;

public class TenantDbContext : DbContext, ITenantDbContext
{
    public TenantDbContext(DbContextOptions<TenantDbContext> options)
        : base(options)
    {
    }
    public DbSet<TenantEntity> Tenants => Set<TenantEntity>();

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

        builder.Entity<TenantEntity>().ToTable("Tenants");
        builder.HasDefaultSchema("Tenant");
    }

    private void ApplyFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : BaseAuditableEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.Deleted == false);
    }
}
