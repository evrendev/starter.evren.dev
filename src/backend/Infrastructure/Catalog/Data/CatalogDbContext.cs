using System.Reflection;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Infrastructure.Catalog.Configurations;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Infrastructure.Catalog.Data;

public class CatalogDbContext : DbContext, ICatalogDbContext
{
    private readonly ILogger<CatalogDbContext> _logger;

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options,
        ILogger<CatalogDbContext> logger) : base(options)
    {
        _logger = logger;
    }

    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public DbSet<Absence> Absences => Set<Absence>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new TodoListConfiguration());
        builder.ApplyConfiguration(new TodoItemConfiguration());
        builder.ApplyConfiguration(new AbsenceConfiguration());

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var entityClrType = entityType.ClrType;

            if (!typeof(BaseAuditableEntity).IsAssignableFrom(entityClrType))
                continue;

            try
            {
                var filter = typeof(CatalogDbContext)
                    .GetMethod(nameof(SoftFilterDelete), BindingFlags.NonPublic | BindingFlags.Static)
                    ?.MakeGenericMethod(entityClrType);

                filter?.Invoke(null, new object[] { builder });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Soft delete filter for {entityClrType.Name} failed.");
            }
        }

        builder.HasDefaultSchema("Catalog");
    }

    private static void SoftFilterDelete<TEntity>(ModelBuilder modelBuilder) where TEntity : BaseAuditableEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => !e.Deleted);
    }
}
