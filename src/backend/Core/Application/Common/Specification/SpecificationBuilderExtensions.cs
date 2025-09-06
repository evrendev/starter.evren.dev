using System.Linq.Expressions;
using System.Reflection;

namespace EvrenDev.Application.Common.Specification;

// See https://github.com/ardalis/Specification/issues/53
public static class SpecificationBuilderExtensions
{
    public static ISpecificationBuilder<T> SearchBy<T>(this ISpecificationBuilder<T> query, BaseFilter filter) =>
        query
            .SearchByKeyword(filter.Search)
            .AdvancedSearch(filter.AdvancedSearch);

    public static ISpecificationBuilder<T> PaginateBy<T>(this ISpecificationBuilder<T> query, PaginationFilter filter)
    {
        if (filter.Page <= 0)
        {
            filter.Page = 1;
        }

        if (filter.ItemsPerPage <= 0)
        {
            filter.ItemsPerPage = 10;
        }

        if (filter.Page > 1)
        {
            query = query.Skip((filter.Page - 1) * filter.ItemsPerPage);
        }

        return query
            .Take(filter.ItemsPerPage)
            .OrderBy(filter.SortBy, filter.SortDesc);
    }

    public static ISpecificationBuilder<T> SearchByKeyword<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        string? keyword) =>
        specificationBuilder.AdvancedSearch(new Search { Keyword = keyword });

    public static ISpecificationBuilder<T> AdvancedSearch<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        Search? search)
    {
        if (!string.IsNullOrEmpty(search?.Keyword))
        {
            if (search.Fields?.Any() is true)
            {
                // search seleted fields (can contain deeper nested fields)
                foreach (var field in search.Fields)
                {
                    var paramExpr = Expression.Parameter(typeof(T));

                    Expression propertyExpr = paramExpr;
                    foreach (var member in field.Split('.'))
                    {
                        propertyExpr = Expression.PropertyOrField(propertyExpr, member);
                    }

                    specificationBuilder.AddSearchPropertyByKeyword(propertyExpr, paramExpr, search.Keyword);
                }
            }
            else
            {
                // search all fields (only first level)
                foreach (var property in typeof(T).GetProperties()
                    .Where(prop => (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType) is { } propertyType
                        && !propertyType.IsEnum
                        && Type.GetTypeCode(propertyType) != TypeCode.Object))
                {
                    var paramExpr = Expression.Parameter(typeof(T));
                    var propertyExpr = Expression.Property(paramExpr, property);

                    specificationBuilder.AddSearchPropertyByKeyword(propertyExpr, paramExpr, search.Keyword);
                }
            }
        }

        return specificationBuilder;
    }

    private static void AddSearchPropertyByKeyword<T>(this ISpecificationBuilder<T> specificationBuilder, Expression propertyExpr, ParameterExpression paramExpr, string keyword)
    {
        if (propertyExpr is not MemberExpression memberExpr || memberExpr.Member is not PropertyInfo property)
        {
            throw new ArgumentException("propertyExpr must be a property expression.", nameof(propertyExpr));
        }

        // Generate lambda [ x => x.Property ] for string properties
        // or [ x => ((object)x.Property) is null ? null : x.Property.ToString() ] for other properties
        var selectorExpr =
            property.PropertyType == typeof(string)
                ? propertyExpr
                : Expression.Condition(
                    Expression.Equal(
                        Expression.Convert(propertyExpr, typeof(object)),
                        Expression.Constant(null, typeof(object))),
                    Expression.Constant(null, typeof(string)),
                    Expression.Call(propertyExpr, "ToString", null, null));

        var selector = Expression.Lambda<Func<T, string>>(selectorExpr, paramExpr);

        ((List<SearchExpressionInfo<T>>)specificationBuilder.Specification.SearchCriterias)
            .Add(new SearchExpressionInfo<T>(selector!, $"%{keyword}%", 1));
    }

    public static ISpecificationBuilder<T> OrderBy<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        string[]? sortByFields,
        string? sortDirection)
    {
        if (sortByFields is null || !sortByFields.Any())
        {
            return specificationBuilder;
        }

        bool isDescending = sortDirection?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;

        bool isFirstField = true;

        foreach (var field in sortByFields)
        {
            var paramExpr = Expression.Parameter(typeof(T));
            Expression propertyExpr = paramExpr;
            foreach (var member in field.Split('.'))
            {
                propertyExpr = Expression.PropertyOrField(propertyExpr, member);
            }

            var keySelector = Expression.Lambda<Func<T, object?>>(
                Expression.Convert(propertyExpr, typeof(object)),
                paramExpr);

            OrderTypeEnum orderType;
            if (isFirstField)
            {
                orderType = isDescending ? OrderTypeEnum.OrderByDescending : OrderTypeEnum.OrderBy;
                isFirstField = false;
            }
            else
            {
                orderType = isDescending ? OrderTypeEnum.ThenByDescending : OrderTypeEnum.ThenBy;
            }

            ((List<OrderExpressionInfo<T>>)specificationBuilder.Specification.OrderExpressions)
                .Add(new OrderExpressionInfo<T>(keySelector, orderType));
        }

        return specificationBuilder;
    }
}
