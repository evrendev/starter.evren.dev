using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Specifications;

public class LessonPageProgressByUserAndPageSpec : Specification<LessonPageProgress>, ISingleResultSpecification<LessonPageProgress>
{
    public LessonPageProgressByUserAndPageSpec(string userId, Guid lessonPageId) =>
        Query.Where(p => p.UserId == userId && p.LessonPageId == lessonPageId);
}
