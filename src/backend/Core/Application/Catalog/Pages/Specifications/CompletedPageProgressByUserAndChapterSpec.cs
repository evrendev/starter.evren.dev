using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Specifications;

public class CompletedPageProgressByUserAndChapterSpec : Specification<PageProgress>
{
    public CompletedPageProgressByUserAndChapterSpec(string userId, Guid chapterId) =>
        Query.Where(p => p.UserId == userId && p.Completed && p.Page.ChapterId == chapterId);
}
