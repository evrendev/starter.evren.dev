using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Specifications;

public class PageProgressListByUserAndChapterSpec : Specification<PageProgress>
{
    public PageProgressListByUserAndChapterSpec(string userId, Guid chapterId) =>
        Query.Where(p => p.UserId == userId && p.Page.ChapterId == chapterId);
}
