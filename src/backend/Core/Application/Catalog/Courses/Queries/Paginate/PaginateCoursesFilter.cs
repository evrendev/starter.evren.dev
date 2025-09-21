using EvrenDev.Application.Catalog.Courses.Entities;
using EvrenDev.Application.Catalog.Courses.Specifications;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Courses.Queries.Paginate;

public class PaginateCoursesFilter : PaginationFilter, IRequest<PaginationResponse<CourseDto>>
{
    public Guid? CategoryId { get; set; }
}

public class PaginateCoursesFilterHandler(IReadRepository<Course> repository) : IRequestHandler<PaginateCoursesFilter, PaginationResponse<CourseDto>>
{
    public async Task<PaginationResponse<CourseDto>> Handle(PaginateCoursesFilter request, CancellationToken cancellationToken)
    {
        var spec = new CoursesBySearchRequestWithCategoriesSpec(request);
        return await repository.PaginatedListAsync(spec, request.Page, request.ItemsPerPage, cancellationToken);
    }
}
