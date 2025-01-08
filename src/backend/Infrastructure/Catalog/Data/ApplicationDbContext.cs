using System.Linq.Expressions;
using System.Reflection;
using EvrenDev.Application.Common.Interfaces;

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

        // Apply global query filters for multi-tenancy and soft delete
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var entityClrType = entityType.ClrType;

            // Apply tenant filter
            if (typeof(ITenant).IsAssignableFrom(entityClrType))
            {
                var tenantMethod = typeof(ApplicationDbContext)
                    .GetMethod(nameof(ApplyTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.MakeGenericMethod(entityClrType);
                tenantMethod?.Invoke(this, [builder]);
            }

            // Apply soft delete filter
            if (typeof(BaseAuditableEntity).IsAssignableFrom(entityClrType))
            {
                var softDeleteMethod = typeof(ApplicationDbContext)
                    .GetMethod(nameof(ApplySoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.MakeGenericMethod(entityClrType);
                softDeleteMethod?.Invoke(this, [builder]);
            }
        }

        builder.HasDefaultSchema("Catalog");
    }

    private void ApplyTenantFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : class, ITenant
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.TenantId == _tenantService.GetCurrentTenantId());
    }

    private void ApplySoftDeleteFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : BaseAuditableEntity
    {
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var deletedProperty = Expression.Property(parameter, nameof(BaseAuditableEntity.Deleted));
        var condition = Expression.Equal(deletedProperty, Expression.Constant(false));
        var lambda = Expression.Lambda<Func<TEntity, bool>>(condition, parameter);

        modelBuilder.Entity<TEntity>().HasQueryFilter(lambda);
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
