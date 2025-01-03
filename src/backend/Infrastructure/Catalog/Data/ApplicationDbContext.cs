using System.Reflection;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Services;

namespace EvrenDev.Infrastructure.Catalog.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ITenantService _tenantService;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        ITenantService tenantService) : base(options)
    {
        _tenantService = tenantService;
    }

    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply global query filter for multi-tenancy
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var entityClrType = entityType.ClrType;
            if (typeof(ITenant).IsAssignableFrom(entityClrType))
            {
                var method = typeof(ApplicationDbContext)
                    .GetMethod(nameof(ApplyTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.MakeGenericMethod(entityClrType);
                method?.Invoke(this, new object[] { builder });
            }
        }

        builder.HasDefaultSchema("Catalog");
    }

    private void ApplyTenantFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : class, ITenant
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.TenantId == _tenantService.GetCurrentTenantId());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<ITenant>().Where(e => e.State == EntityState.Added))
        {
            entry.Entity.TenantId = _tenantService.GetCurrentTenantId();
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
