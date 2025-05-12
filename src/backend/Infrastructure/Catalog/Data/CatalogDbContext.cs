using System.Reflection;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Infrastructure.Catalog.Data;

public class CatalogDbContext : DbContext, ICatalogDbContext
{
    private readonly ITenantService _tenantService;
    private readonly ILogger<CatalogDbContext> _logger;

    public CatalogDbContext(
        DbContextOptions<CatalogDbContext> options,
        ITenantService tenantService,
        ILogger<CatalogDbContext> logger) : base(options)
    {
        _tenantService = tenantService;
        _logger = logger;
    }

    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public DbSet<Absence> Absences => Set<Absence>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply global query filters for multi-tenancy and soft delete
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var entityClrType = entityType.ClrType;

            // Apply  filter
            if (typeof(BaseAuditableEntity).IsAssignableFrom(entityClrType))
            {
                var filter = typeof(CatalogDbContext)
                    .GetMethod(nameof(ApplyFilter), BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.MakeGenericMethod(entityClrType);

                filter?.Invoke(this, [builder]);
            }
        }

        builder.HasDefaultSchema("Catalog");
    }

    private void ApplyFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : BaseAuditableEntity
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.Deleted == false && e.TenantId == tenantId);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<ITenant>().Where(e => e.State == EntityState.Added))
        {
            var tenantId = _tenantService.GetCurrentTenantId();
            _logger.LogInformation("Setting tenant ID {TenantId} for new catalog entity of type {EntityType}",
                tenantId, entry.Entity.GetType().Name);
            entry.Entity.TenantId = tenantId;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
