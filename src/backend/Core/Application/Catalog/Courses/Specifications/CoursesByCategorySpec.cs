using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Courses.Specifications;

public class CoursesByCategorySpec : Specification<Course>
{
    public CoursesByCategorySpec(Guid categoryId) =>
        Query.Where(p => p.CategoryId == categoryId);
}
