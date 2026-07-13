using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Specifications;

public class LessonProgressByUserAndLessonSpec : Specification<LessonProgress>, ISingleResultSpecification<LessonProgress>
{
    public LessonProgressByUserAndLessonSpec(string userId, Guid lessonId) =>
        Query.Where(p => p.UserId == userId && p.LessonId == lessonId);
}
