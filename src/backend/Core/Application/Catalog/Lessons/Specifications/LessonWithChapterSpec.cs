using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Lessons.Specifications;

public class LessonWithChapterSpec : Specification<Lesson>, ISingleResultSpecification<Lesson>
{
    public LessonWithChapterSpec(Guid lessonId) =>
        Query
            .Where(l => l.Id == lessonId)
            .Include(l => l.Chapter);
}
