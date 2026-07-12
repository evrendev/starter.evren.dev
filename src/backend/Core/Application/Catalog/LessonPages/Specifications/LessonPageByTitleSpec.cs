using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.LessonPages.Specifications;

public class LessonPageByTitleSpec : Specification<LessonPage>, ISingleResultSpecification<LessonPage>
{
    public LessonPageByTitleSpec(string title) =>
        Query.Where(p => p.Title == title);
}
