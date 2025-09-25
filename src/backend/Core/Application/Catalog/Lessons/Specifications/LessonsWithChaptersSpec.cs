using EvrenDev.Application.Catalog.Lessons.Entities;
using EvrenDev.Application.Catalog.Lessons.Queries.Get;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Lessons.Specifications;

public class LessonsWithChaptersSpec : Specification<Lesson, LessonExportDto>
{
    public LessonsWithChaptersSpec(GetAllLessonsRequest request)
    {
        Query.Include(p => p.Chapter)
            .OrderBy(c => c.Title);
    }
}
