using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Lessons.Specifications;

public class LessonByTitleSpec : Specification<Lesson>, ISingleResultSpecification<Lesson>
{
    public LessonByTitleSpec(string title) =>
        Query.Where(p => p.Title == title);
}
