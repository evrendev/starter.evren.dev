using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Common.Models;

public class PaginatedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int ItemsLength { get; }

    public PaginatedList(IReadOnlyCollection<T> items, int totalCount)
    {
        Items = items;
        ItemsLength = totalCount;
    }

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source,
        int page,
        int itemsPerPage)
    {
        var totalCount = await source.CountAsync();

        // Apply pagination
        var items = await source
            .Skip((page - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .ToListAsync();

        return new PaginatedList<T>(items, totalCount);
    }
}
