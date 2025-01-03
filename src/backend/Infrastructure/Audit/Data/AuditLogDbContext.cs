namespace EvrenDev.Infrastructure.Audit.Data;

public class AuditLogDbContext : DbContext
{
    public AuditLogDbContext(DbContextOptions<AuditLogDbContext> options) : base(options)
    {
    }

    public DbSet<AuditLog>? AuditLogs { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AuditLog>().ToTable("AuditLogs");
        modelBuilder.HasDefaultSchema("Audit");
    }
}
