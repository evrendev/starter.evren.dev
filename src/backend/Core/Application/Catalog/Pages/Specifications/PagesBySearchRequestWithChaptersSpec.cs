using EvrenDev.Application.Catalog.Pages.Entities;
using EvrenDev.Application.Catalog.Pages.Queries.Paginate;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Specifications;

public class PagesBySearchRequestWithChaptersSpec : Specification<Page, PageDto>
{
    public PagesBySearchRequestWithChaptersSpec(PaginatePagesFilter request)
    {
        Query.Include(p => p.Chapter)
            .Where(page =>
                (
                    !request.ChapterId.HasValue
                    ||
                    page.ChapterId.Equals(request.ChapterId!.Value)
                )
                &&
                (
                    string.IsNullOrEmpty(request.Search)
                    ||
                    page.Title.ToLower().Contains(request.Search.ToLower())
                    ||
                    page.Chapter.Title.ToLower().Contains(request.Search.ToLower())
                )
            )
            .OrderBy(p => p.Order, !request.HasOrderBy())
            .PaginateBy(request);
    }
}
