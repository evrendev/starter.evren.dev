namespace EvrenDev.Application.Common.Models;

public static class PaginationFilterExtensions
{
    public static bool HasOrderBy(this PaginationFilter filter) =>
        filter.SortBy?.Any() is true;
}
