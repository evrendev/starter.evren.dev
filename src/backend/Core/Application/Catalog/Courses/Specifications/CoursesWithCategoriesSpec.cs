using EvrenDev.Application.Catalog.Courses.Entities;
using EvrenDev.Application.Catalog.Courses.Queries.Get;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Courses.Specifications;

public class CoursesWithCategoriesSpec : Specification<Course, CourseExportDto>
{
    public CoursesWithCategoriesSpec(GetAllCoursesRequest request)
    {
        Query.Include(p => p.Category)
            .Where(course => course.Published)
            .OrderBy(c => c.Title);
    }
}
