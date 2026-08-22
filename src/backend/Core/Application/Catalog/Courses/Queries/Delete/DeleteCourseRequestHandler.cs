using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Courses.Queries.Delete;

public class DeleteCourseRequest(Guid id) : IRequest<Guid>
{
    public Guid Id { get; set; } = id;
}

public class DeleteCourseRequestHandler(IRepositoryWithEvents<Course> repository, IStringLocalizer<DeleteCourseRequestHandler> localizer)
    : IRequestHandler<DeleteCourseRequest, Guid>
{
    public async Task<Guid> Handle(DeleteCourseRequest request, CancellationToken cancellationToken)
    {
        var course = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = course ?? throw new NotFoundException(localizer["catalog.courses.delete.notfound"]);

        // Add Domain Events automatically by using IRepositoryWithEvents
        await repository.DeleteAsync(course, cancellationToken);

        return request.Id;
    }
}
