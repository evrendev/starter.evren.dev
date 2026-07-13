using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Specifications;

public class CompletedLessonPageProgressByUserAndLessonSpec : Specification<LessonPageProgress>
{
    public CompletedLessonPageProgressByUserAndLessonSpec(string userId, Guid lessonId) =>
        Query.Where(p => p.UserId == userId && p.Completed && p.LessonPage.LessonId == lessonId);
}
