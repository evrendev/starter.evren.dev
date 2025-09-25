using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;

namespace EvrenDev.Application.Catalog.Lessons.Queries.Delete;

public class DeleteLessonRequest(Guid id) : IRequest<Guid>
{
    public Guid Id { get; set; } = id;
}

public class DeleteLessonRequestHandler(IRepository<Lesson> repository, IStringLocalizer<DeleteLessonRequestHandler> localizer)
    : IRequestHandler<DeleteLessonRequest, Guid>
{
    public async Task<Guid> Handle(DeleteLessonRequest request, CancellationToken cancellationToken)
    {
        var lesson = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = lesson ?? throw new NotFoundException(localizer["catalog.lessons.delete.notfound"]);

        lesson.DomainEvents.Add(EntityDeletedEvent.WithEntity(lesson));

        await repository.DeleteAsync(lesson, cancellationToken);

        return request.Id;
    }
}
