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
                    course.Title.Contains(request.Search)
                    ||
                    course.Category.Name.Contains(request.Search)
                    ||
                    (
                        course.Description != null
                        &&
                        course.Description.Contains(request.Search)
                    )
                    ||
                    (
                        course.Introduction != null
                        &&
                        course.Introduction.Contains(request.Search)
                    )
                    ||
                    (
                        course.PreviewVideoUrl != null
                        &&
                        course.PreviewVideoUrl.Contains(request.Search)
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
