using EvrenDev.Application.Catalog.Courses.Entities;
using EvrenDev.Application.Catalog.Courses.Queries.Paginate;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Courses.Specifications;

public class CoursesBySearchRequestWithCategoriesSpec : EntitiesByPaginationFilterSpec<Course, CourseDto>
{
    public CoursesBySearchRequestWithCategoriesSpec(PaginateCoursesFilter request)
        : base(request) =>
        Query
            .Include(p => p.Category)
            .OrderBy(c => c.Title, !request.HasOrderBy())
            .Where(p => p.CategoryId.Equals(request.CategoryId!.Value), request.CategoryId.HasValue);
}
