using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Chapters.Specifications;

public class ChaptersByCourseSpec : Specification<Chapter>
{
    public ChaptersByCourseSpec(Guid courseId) =>
        Query.Where(p => p.CourseId == courseId);
}
