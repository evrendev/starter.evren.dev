using EvrenDev.Application.Catalog.Courses.Entities;
using EvrenDev.Application.Catalog.Courses.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Courses.Queries.Get;

public class GetCourseRequest(Guid id) : IRequest<CourseDetailsDto>
{
    public Guid Id { get; set; } = id;
}

public class GetCourseRequestHandler(IRepository<Course> repository, IStringLocalizer<GetCourseRequestHandler> localizer)
    : IRequestHandler<GetCourseRequest, CourseDetailsDto>
{
    public async Task<CourseDetailsDto> Handle(GetCourseRequest request, CancellationToken cancellationToken) =>
        await repository.FirstOrDefaultAsync(
            new CourseByIdWithCategorySpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(localizer["catalog.courses.get.notfound"], request.Id));
}
