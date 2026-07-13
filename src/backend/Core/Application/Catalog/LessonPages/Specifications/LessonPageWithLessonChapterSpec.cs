using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Specifications;

public class LessonPageWithLessonChapterSpec : Specification<LessonPage>, ISingleResultSpecification<LessonPage>
{
    public LessonPageWithLessonChapterSpec(Guid lessonPageId) =>
        Query
            .Where(p => p.Id == lessonPageId)
            .Include(p => p.Lesson)
            .ThenInclude(l => l.Chapter);
}
