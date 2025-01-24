using EvrenDev.Application.Common.Interfaces;

namespace EvrenDev.Infrastructure.Audit.Data;

public class AuditLogDbContext : DbContext, IAuditLogDbContext
{
    public AuditLogDbContext(DbContextOptions<AuditLogDbContext> options) : base(options)
    {
    }
    public DbSet<AuditLog> Audits => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AuditLog>().ToTable("AuditLogs");
        modelBuilder.HasDefaultSchema("Audit");
    }
}
