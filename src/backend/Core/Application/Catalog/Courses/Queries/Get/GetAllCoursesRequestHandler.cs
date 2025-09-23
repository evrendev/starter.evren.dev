using EvrenDev.Application.Catalog.Courses.Entities;
using EvrenDev.Application.Catalog.Courses.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Courses.Queries.Get;

public class GetAllCoursesRequest() : IRequest<List<CourseExportDto>>
{
}

public class GetAllCoursesRequestHandler(IRepository<Course> repository, IStringLocalizer<GetAllCoursesRequestHandler> localizer)
    : IRequestHandler<GetAllCoursesRequest, List<CourseExportDto>>
{
    public async Task<List<CourseExportDto>> Handle(GetAllCoursesRequest request, CancellationToken cancellationToken)
    {
        var courses = await repository.ListAsync(new CoursesWithCategoriesSpec(request), cancellationToken);

        if (courses == null || !courses.Any())
            throw new NotFoundException(string.Format(localizer["catalog.courses.list.notfound"]));

        return courses;
    }
}
