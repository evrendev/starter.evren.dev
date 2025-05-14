using System.Reflection;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Entities.Tenant;
using EvrenDev.Infrastructure.Catalog.Configurations;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Infrastructure.Catalog.Data;

public class CatalogDbContext : MultiTenantDbContext, ICatalogDbContext
{
    private readonly ILogger<CatalogDbContext> _logger;

    public CatalogDbContext(IMultiTenantContextAccessor<TenantEntity> multiTenantContextAccessor,
        DbContextOptions<CatalogDbContext> options,
        ILogger<CatalogDbContext> logger) : base(multiTenantContextAccessor, options)
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

            if (typeof(BaseAuditableEntity).IsAssignableFrom(entityClrType))
            {
                var filter = typeof(CatalogDbContext)
                    .GetMethod(nameof(SoftFilterDelete), BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.MakeGenericMethod(entityClrType);

                filter?.Invoke(this, [builder]);
            }
        }

        builder.HasDefaultSchema("Catalog");
    }

    private static void SoftFilterDelete<TEntity>(ModelBuilder modelBuilder) where TEntity : BaseAuditableEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.Deleted == false);
    }
}
