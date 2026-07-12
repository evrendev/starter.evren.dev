using EvrenDev.Application.Catalog.LessonPages.Entities;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Specifications;

public class LessonPagesWithLessonsSpec : Specification<LessonPage, LessonPageExportDto>
{
    public LessonPagesWithLessonsSpec() =>
        Query
            .Include(p => p.Lesson)
            .OrderBy(p => p.Order);
}
