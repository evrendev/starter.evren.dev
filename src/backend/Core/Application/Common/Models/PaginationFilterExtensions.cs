namespace EvrenDev.Application.Common.Models;

public static class PaginationFilterExtensions
{
    public static bool HasOrderBy(this PaginationFilter filter)
    {
        return filter.SortBy?.Any() is true;
    }
}
