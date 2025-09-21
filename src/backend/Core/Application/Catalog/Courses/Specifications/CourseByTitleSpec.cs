using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Courses.Specifications;

public class CourseByTitleSpec : Specification<Course>, ISingleResultSpecification<Course>
{
    public CourseByTitleSpec(string title) =>
        Query.Where(p => p.Title == title);
}
