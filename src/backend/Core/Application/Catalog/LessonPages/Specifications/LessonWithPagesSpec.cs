using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Specifications;

public class LessonWithPagesSpec : Specification<Lesson>, ISingleResultSpecification<Lesson>
{
    public LessonWithPagesSpec(Guid lessonId) =>
        Query
            .Where(l => l.Id == lessonId)
            .Include(l => l.Pages);
}
