using Microsoft.AspNetCore.Mvc;

namespace EvrenDev.Application.Common.Models;

public class PaginationFilter : BaseFilter
{
    public int Page { get; set; }

    public int ItemsPerPage { get; set; } = int.MaxValue;

    public List<SortBy>? SortBy { get; set; }
}

public class SortBy
{
    public string? Key { get; set; }
    public string? Order { get; set; }
}
