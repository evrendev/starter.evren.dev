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

        builder.Entity<TenantEntity>().ToTable("Tenants");
        builder.HasDefaultSchema("Tenant");
    }
}
