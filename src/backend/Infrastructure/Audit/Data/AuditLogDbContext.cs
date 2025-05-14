using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Entities.Tenant;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;

namespace EvrenDev.Infrastructure.Audit.Data;

public class AuditLogDbContext : MultiTenantDbContext, IAuditLogDbContext
{
    public AuditLogDbContext(IMultiTenantContextAccessor<TenantEntity> multiTenantContextAccessor, DbContextOptions<AuditLogDbContext> options) : base(multiTenantContextAccessor, options)
    {
    }
    public DbSet<AuditLog> Audits => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AuditLog>().ToTable("AuditLogs").IsMultiTenant();
        builder.HasDefaultSchema("Audit");
    }
}
