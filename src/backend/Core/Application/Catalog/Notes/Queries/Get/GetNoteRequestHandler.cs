using EvrenDev.Application.Catalog.Notes.Entities;
using EvrenDev.Application.Catalog.Notes.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Notes.Queries.Get;

public class GetNoteRequest(Guid id) : IRequest<NoteDto>
{
    public Guid Id { get; set; } = id;
}

public class GetNoteRequestHandler(IRepository<Note> repository, IStringLocalizer<GetNoteRequestHandler> localizer)
    : IRequestHandler<GetNoteRequest, NoteDto>
{
    public async Task<NoteDto> Handle(GetNoteRequest request, CancellationToken cancellationToken) =>
        await repository.FirstOrDefaultAsync(
            new NoteByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(localizer["catalog.notes.get.notfound"], request.Id));
}

public class GetNotesByPageRequest(Guid pageId) : IRequest<List<NoteDto>>
{
    public Guid PageId { get; set; } = pageId;
}

public class GetNotesByPageRequestHandler(IReadRepository<Note> repository)
    : IRequestHandler<GetNotesByPageRequest, List<NoteDto>>
{
    public async Task<List<NoteDto>> Handle(GetNotesByPageRequest request, CancellationToken cancellationToken)
    {
        var spec = new NotesByPageSpec(request.PageId);
        return await repository.ListAsync(spec, cancellationToken);
    }
}
