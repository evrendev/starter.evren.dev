using System.Reflection;
using EvrenDev.Domain.Entities.Tenant;

namespace EvrenDev.Infrastructure.Tenant.Data;

public class TenantDbContext : DbContext
{
    public TenantDbContext(DbContextOptions<TenantDbContext> options)
        : base(options)
    {
    }
    public DbSet<TenantEntity>? Tenants { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TenantEntity>().ToTable("Tenants");
        builder.HasDefaultSchema("Tenant");
    }
}
