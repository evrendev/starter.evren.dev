using EvrenDev.Application.Catalog.Courses.Entities;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using Mapster;

namespace EvrenDev.Application.Catalog.Courses.Queries.Get;

public class GetAllCoursesRequest() : IRequest<List<CourseDto>>
{
}

public class GetAllCoursesRequestHandler(IRepository<Course> repository, IStringLocalizer<GetAllCoursesRequestHandler> localizer)
    : IRequestHandler<GetAllCoursesRequest, List<CourseDto>>
{
    public async Task<List<CourseDto>> Handle(GetAllCoursesRequest request, CancellationToken cancellationToken)
    {
        var courses = await repository.ListAsync(cancellationToken);

        if (courses == null || !courses.Any())
            throw new NotFoundException(string.Format(localizer["catalog.courses.list.notfound"]));

        return courses.Adapt<List<CourseDto>>();
    }
}
