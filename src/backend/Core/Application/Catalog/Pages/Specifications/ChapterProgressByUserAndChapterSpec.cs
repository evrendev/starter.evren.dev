using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Specifications;

public class ChapterProgressByUserAndChapterSpec : Specification<ChapterProgress>, ISingleResultSpecification<ChapterProgress>
{
    public ChapterProgressByUserAndChapterSpec(string userId, Guid chapterId) =>
        Query.Where(p => p.UserId == userId && p.ChapterId == chapterId);
}
