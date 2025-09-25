using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Lessons.Specifications;

public class LessonsByChapterSpec : Specification<Lesson>
{
    public LessonsByChapterSpec(Guid chapterId) =>
        Query.Where(p => p.ChapterId == chapterId);
}
