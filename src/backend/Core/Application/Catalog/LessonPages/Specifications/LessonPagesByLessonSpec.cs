using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Specifications;

public class LessonPagesByLessonSpec : Specification<LessonPage>
{
    public LessonPagesByLessonSpec(Guid lessonId) =>
        Query
            .Where(p => p.LessonId == lessonId)
            .Include(p => p.Lesson)
            .OrderBy(p => p.Order);
}
