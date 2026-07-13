using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Lessons.Specifications;

public class LessonsByCourseSpec : Specification<Lesson>
{
    public LessonsByCourseSpec(Guid courseId) =>
        Query.Where(l => l.Chapter.CourseId == courseId);
}
