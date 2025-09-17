using EvrenDev.Application.Catalog.Courses.Entities;
using EvrenDev.Application.Catalog.Courses.Specifications;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Courses.Queries.Search;

public class SearchCoursesRequest : PaginationFilter, IRequest<PaginationResponse<CourseDto>>
{
    public Guid? CategoryId { get; set; }
}

public class SearchCoursesRequestHandler(IReadRepository<Course> repository)
    : IRequestHandler<SearchCoursesRequest, PaginationResponse<CourseDto>>
{
    public async Task<PaginationResponse<CourseDto>> Handle(SearchCoursesRequest request, CancellationToken cancellationToken)
    {
        var spec = new CoursesBySearchRequestWithCategoriesSpec(request);
        return await repository.PaginatedListAsync(spec, request.Page, request.ItemsPerPage, cancellationToken: cancellationToken);
    }
}
