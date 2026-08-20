using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace EvrenDev.Infrastructure.Persistence.Context;

internal static class ModelBuilderExtensions
{
    // EF Core 10 / Finbuckle v10 (Task S3): named query filters replaced the old single/
    // anonymous HasQueryFilter() — Finbuckle's own IsMultiTenant() now applies its own
    // named filter (keyed "TenantToken") per entity, and EF Core forbids mixing a named
    // filter with an anonymous one on the same entity. So this no longer reads and
    // combines any existing filter — it just registers its own independently-named
    // filter; EF Core ANDs every named filter on an entity together automatically. See
    // Finbuckle's EFCore.md docs and https://learn.microsoft.com/ef/core/querying/filters#using-multiple-query-filters.
    public static ModelBuilder AppendGlobalQueryFilter<TInterface>(this ModelBuilder modelBuilder,
        string filterKey, Expression<Func<TInterface, bool>> filter)
    {
        // get a list of entities without a baseType that implement the interface TInterface
        var entities = modelBuilder.Model.GetEntityTypes()
            .Where(e => e.BaseType is null && e.ClrType.GetInterface(typeof(TInterface).Name) is not null)
            .Select(e => e.ClrType);

        foreach (var entity in entities)
        {
            var parameterType = Expression.Parameter(modelBuilder.Entity(entity).Metadata.ClrType);
            var filterBody = ReplacingExpressionVisitor.Replace(filter.Parameters.Single(), parameterType, filter.Body);

            modelBuilder.Entity(entity).HasQueryFilter(filterKey, Expression.Lambda(filterBody, parameterType));
        }

        return modelBuilder;
    }
}
