using EvrenDev.Application.Catalog.Courses.Entities;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Courses.Specifications;

public class CourseByIdWithCategorySpec : Specification<Course, CourseDetailsDto>, ISingleResultSpecification<Course>
{
    public CourseByIdWithCategorySpec(Guid id) =>
        Query
            .Where(p => p.Id == id)
            .Include(p => p.Category);
}
