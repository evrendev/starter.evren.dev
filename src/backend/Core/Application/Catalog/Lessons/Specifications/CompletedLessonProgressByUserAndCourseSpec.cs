using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Lessons.Specifications;

public class CompletedLessonProgressByUserAndCourseSpec : Specification<LessonProgress>
{
    public CompletedLessonProgressByUserAndCourseSpec(string userId, Guid courseId) =>
        Query.Where(p => p.UserId == userId
            && p.Status == ProgressStatus.Completed
            && p.Lesson.Chapter.CourseId == courseId);
}
