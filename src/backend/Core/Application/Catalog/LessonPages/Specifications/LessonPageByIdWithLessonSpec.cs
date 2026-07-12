using EvrenDev.Application.Catalog.LessonPages.Entities;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Specifications;

public class LessonPageByIdWithLessonSpec : Specification<LessonPage, LessonPageDetailsDto>, ISingleResultSpecification<LessonPage>
{
    public LessonPageByIdWithLessonSpec(Guid id) =>
        Query
            .Where(p => p.Id == id)
            .Include(p => p.Lesson);
}
