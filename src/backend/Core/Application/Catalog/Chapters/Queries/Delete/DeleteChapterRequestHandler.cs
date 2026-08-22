using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Chapters.Queries.Delete;

public class DeleteChapterRequest(Guid id) : IRequest<Guid>
{
    public Guid Id { get; set; } = id;
}

public class DeleteChapterRequestHandler(IRepositoryWithEvents<Chapter> repository, IStringLocalizer<DeleteChapterRequestHandler> localizer)
    : IRequestHandler<DeleteChapterRequest, Guid>
{
    public async Task<Guid> Handle(DeleteChapterRequest request, CancellationToken cancellationToken)
    {
        var chapter = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = chapter ?? throw new NotFoundException(localizer["catalog.chapters.delete.notfound"]);

        // Add Domain Events automatically by using IRepositoryWithEvents
        await repository.DeleteAsync(chapter, cancellationToken);

        return request.Id;
    }
}
