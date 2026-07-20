using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Chapters.Specifications;

public class CompletedChapterProgressByUserAndCourseSpec : Specification<ChapterProgress>
{
    public CompletedChapterProgressByUserAndCourseSpec(string userId, Guid courseId) =>
        Query.Where(p => p.UserId == userId
            && p.Status == ProgressStatus.Completed
            && p.Chapter.CourseId == courseId);
}
