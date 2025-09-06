namespace EvrenDev.Application.Common.Models;

public class PaginationFilter : BaseFilter
{
    public int Page { get; set; }

    public int ItemsPerPage { get; set; } = int.MaxValue;

    public string[]? SortBy { get; set; }

    public string? SortDesc { get; set; }
}
