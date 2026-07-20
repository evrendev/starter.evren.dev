using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Specifications;

public class PageProgressByUserAndPageSpec : Specification<PageProgress>, ISingleResultSpecification<PageProgress>
{
    public PageProgressByUserAndPageSpec(string userId, Guid pageId) =>
        Query.Where(p => p.UserId == userId && p.PageId == pageId);
}
