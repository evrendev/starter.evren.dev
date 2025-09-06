namespace EvrenDev.Application.Common.Models;

public class PaginationResponse<T>(List<T> items, int total, int page, int itemsPerPage)
{
    public List<T> Items { get; set; } = items;

    public int Page { get; set; } = page;

    public int TotalPages { get; set; } = (int)Math.Ceiling(total / (double)itemsPerPage);

    public int Total { get; set; } = total;

    public int ItemsPerPage { get; set; } = itemsPerPage;

    public bool HasPreviousPage => Page > 1;

    public bool HasNextPage => Page < TotalPages;
}
