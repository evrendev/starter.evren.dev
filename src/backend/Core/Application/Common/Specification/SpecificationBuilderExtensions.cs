using System.Linq.Expressions;
using System.Reflection;

namespace EvrenDev.Application.Common.Specification;

// See https://github.com/ardalis/Specification/issues/53
public static class SpecificationBuilderExtensions
{
    public static ISpecificationBuilder<T> SearchBy<T>(this ISpecificationBuilder<T> query, BaseFilter filter)
        where T : class
    {
        return query
            .SearchByKeyword(filter.Search)
            .AdvancedSearch(filter.AdvancedSearch);
    }

    public static ISpecificationBuilder<T> PaginateBy<T>(this ISpecificationBuilder<T> query, PaginationFilter filter)
    {
        if (filter.Page <= 0) filter.Page = 1;

        if (filter.ItemsPerPage <= 0) filter.ItemsPerPage = 10;

        if (filter.Page > 1) query = query.Skip((filter.Page - 1) * filter.ItemsPerPage);

        return query
            .Take(filter.ItemsPerPage)
            .OrderBy(filter.SortBy);
    }

    public static ISpecificationBuilder<T> SearchByKeyword<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        string? keyword)
        where T : class
    {
        return specificationBuilder.AdvancedSearch(new Search { Keyword = keyword });
    }

    public static ISpecificationBuilder<T> AdvancedSearch<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        Search? search)
        where T : class
    {
        if (!string.IsNullOrEmpty(search?.Keyword))
        {
            if (search.Fields?.Any() is true)
                foreach (var field in search.Fields)
                {
                    var paramExpr = Expression.Parameter(typeof(T));

                    Expression propertyExpr = paramExpr;
                    foreach (var member in field.Split('.'))
                        propertyExpr = Expression.PropertyOrField(propertyExpr, member);

                    specificationBuilder.AddSearchPropertyByKeyword(propertyExpr, paramExpr, search.Keyword);
                }
            else
                foreach (var property in typeof(T).GetProperties()
                             .Where(prop => (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType) is
                                            { } propertyType
                                            && !propertyType.IsEnum
                                            && Type.GetTypeCode(propertyType) != TypeCode.Object))
                {
                    var paramExpr = Expression.Parameter(typeof(T));
                    var propertyExpr = Expression.Property(paramExpr, property);

                    specificationBuilder.AddSearchPropertyByKeyword(propertyExpr, paramExpr, search.Keyword);
                }
        }

        return specificationBuilder;
    }

    private static void AddSearchPropertyByKeyword<T>(this ISpecificationBuilder<T> specificationBuilder,
        Expression propertyExpr, ParameterExpression paramExpr, string keyword)
        where T : class
    {
        if (propertyExpr is not MemberExpression memberExpr || memberExpr.Member is not PropertyInfo property)
            throw new ArgumentException("propertyExpr must be a property expression.", nameof(propertyExpr));

        var selectorExpr =
            property.PropertyType == typeof(string)
                ? propertyExpr
                : Expression.Condition(
                    Expression.Equal(
                        Expression.Convert(propertyExpr, typeof(object)),
                        Expression.Constant(null, typeof(object))),
                    Expression.Constant(null, typeof(string)),
                    Expression.Call(propertyExpr, "ToString", null, null));

        var selector = Expression.Lambda<Func<T, string?>>(selectorExpr, paramExpr);

        // Use the proper Search method from Ardalis.Specification
        specificationBuilder.Search(selector, $"%{keyword}%");
    }

    public static ISpecificationBuilder<T> OrderBy<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        List<SortBy>? sortBy)
    {
        if (sortBy is null || sortBy.Count == 0)
        {
            return specificationBuilder;
        }

        var isFirstField = true;
        IOrderedSpecificationBuilder<T>? orderedBuilder = null;

        foreach (var sortItem in sortBy)
        {
            if (string.IsNullOrWhiteSpace(sortItem.Key))
                continue;

            var isDescending = sortItem.Order?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;

            var paramExpr = Expression.Parameter(typeof(T));
            Expression propertyExpr = paramExpr;
            foreach (var member in sortItem.Key.Split('.'))
                propertyExpr = Expression.PropertyOrField(propertyExpr, member);

            var keySelector = Expression.Lambda<Func<T, object?>>(
                Expression.Convert(propertyExpr, typeof(object)),
                paramExpr);

            if (isFirstField)
            {
                orderedBuilder = isDescending
                    ? specificationBuilder.OrderByDescending(keySelector)
                    : specificationBuilder.OrderBy(keySelector);
                isFirstField = false;
            }
            else
            {
                orderedBuilder = isDescending
                    ? orderedBuilder!.ThenByDescending(keySelector)
                    : orderedBuilder!.ThenBy(keySelector);
            }
        }

        return orderedBuilder ?? specificationBuilder;
    }
}
