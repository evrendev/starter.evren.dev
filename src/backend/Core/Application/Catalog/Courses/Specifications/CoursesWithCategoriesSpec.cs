using EvrenDev.Application.Catalog.Courses.Entities;
using EvrenDev.Application.Catalog.Courses.Queries.Paginate;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Courses.Specifications;

public class CoursesBySearchRequestWithCategoriesSpec : Specification<Course, CourseDto>
{
    public CoursesBySearchRequestWithCategoriesSpec(PaginateCoursesFilter request)
    {
        Query.Include(p => p.Category)
            .Where(course =>
                (
                    !request.CategoryId.HasValue
                    ||
                    course.CategoryId.Equals(request.CategoryId!.Value)
                )
                &&
                (
                    string.IsNullOrEmpty(request.Search)
                    ||
                    course.Title.ToLower().Contains(request.Search.ToLower())
                    ||
                    course.Category.Title.ToLower().Contains(request.Search.ToLower())
                    ||
                    (
                        course.Description != null
                        &&
                        course.Description.ToLower().Contains(request.Search.ToLower())
                    )
                    ||
                    (
                        course.Introduction != null
                        &&
                        course.Introduction.ToLower().Contains(request.Search.ToLower())
                    )
                    ||
                    (
                        course.PreviewVideoUrl != null
                        &&
                        course.PreviewVideoUrl.ToLower().Contains(request.Search.ToLower())
                    )
                )
                &&
                (
                    !request.Published.HasValue
                    ||
                    course.Published == request.Published
                )
            )
            .OrderBy(c => c.Title, !request.HasOrderBy())
            .PaginateBy(request);
    }
}
