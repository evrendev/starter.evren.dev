using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events.Entity;

namespace EvrenDev.Application.Catalog.Chapters.Queries.Delete;

public class DeleteChapterRequest(Guid id) : IRequest<Guid>
{
    public Guid Id { get; set; } = id;
}

public class DeleteChapterRequestHandler(IRepository<Chapter> repository, IStringLocalizer<DeleteChapterRequestHandler> localizer)
    : IRequestHandler<DeleteChapterRequest, Guid>
{
    public async Task<Guid> Handle(DeleteChapterRequest request, CancellationToken cancellationToken)
    {
        var chapter = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = chapter ?? throw new NotFoundException(localizer["catalog.chapters.delete.notfound"]);

        chapter.DomainEvents.Add(EntityDeletedEvent.WithEntity(chapter));

        await repository.DeleteAsync(chapter, cancellationToken);

        return request.Id;
    }
}
