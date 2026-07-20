using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Specifications;

public class PageWithChapterSpec : Specification<Page>, ISingleResultSpecification<Page>
{
    public PageWithChapterSpec(Guid pageId) =>
        Query
            .Where(p => p.Id == pageId)
            .Include(p => p.Chapter);
}
