using EvrenDev.Application.Catalog.Notes.Entities;
using EvrenDev.Application.Catalog.Notes.Queries.Create;
using EvrenDev.Application.Catalog.Notes.Queries.Delete;
using EvrenDev.Application.Catalog.Notes.Queries.Get;

namespace EvrenDev.PublicApi.Controllers.Catalog;

public class NotesController : VersionedApiController
{
    [HttpGet]
    [Authorize]
    [OpenApiOperation("Get notes for a lesson page.", "")]
    public async Task<ApiResponse<List<NoteDto>>> GetByLessonPageAsync([FromQuery] Guid lessonPageId)
    {
        var data = await Mediator.Send(new GetNotesByLessonPageRequest(lessonPageId));

        return ApiResponse<List<NoteDto>>.Success(data);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [OpenApiOperation("Get note details.", "")]
    public async Task<ApiResponse<NoteDto>> GetAsync(Guid id)
    {
        var data = await Mediator.Send(new GetNoteRequest(id));

        if (data == null)
            throw new NotFoundException($"Note with ID '{id}' not found.");

        return ApiResponse<NoteDto>.Success(data);
    }

    [HttpPost]
    [Authorize]
    [OpenApiOperation("Create a new note.", "")]
    public Task<Guid> CreateAsync(CreateNoteRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    [OpenApiOperation("Delete a note.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteNoteRequest(id));
    }
}
