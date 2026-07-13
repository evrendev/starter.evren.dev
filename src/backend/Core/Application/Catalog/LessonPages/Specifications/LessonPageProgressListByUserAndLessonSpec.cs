using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Specifications;

public class LessonPageProgressListByUserAndLessonSpec : Specification<LessonPageProgress>
{
    public LessonPageProgressListByUserAndLessonSpec(string userId, Guid lessonId) =>
        Query.Where(p => p.UserId == userId && p.LessonPage.LessonId == lessonId);
}
