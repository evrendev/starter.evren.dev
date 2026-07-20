using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Specifications;

public class PagesByChapterSpec : Specification<Page>
{
    public PagesByChapterSpec(Guid chapterId) =>
        Query
            .Where(p => p.ChapterId == chapterId)
            .Include(p => p.Chapter)
            .OrderBy(p => p.Order);
}
