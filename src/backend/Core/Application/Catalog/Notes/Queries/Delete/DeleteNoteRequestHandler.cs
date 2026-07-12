using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Notes.Queries.Delete;

public class DeleteNoteRequest(Guid id) : IRequest<Guid>
{
    public Guid Id { get; set; } = id;
}

public class DeleteNoteRequestHandler(IRepository<Note> repository, IStringLocalizer<DeleteNoteRequestHandler> localizer)
    : IRequestHandler<DeleteNoteRequest, Guid>
{
    public async Task<Guid> Handle(DeleteNoteRequest request, CancellationToken cancellationToken)
    {
        var note = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = note ?? throw new NotFoundException(localizer["catalog.notes.delete.notfound"]);

        await repository.DeleteAsync(note, cancellationToken);

        return request.Id;
    }
}
