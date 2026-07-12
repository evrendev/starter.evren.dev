using EvrenDev.Application.Catalog.Notes.Entities;
using EvrenDev.Application.Catalog.Notes.Queries.Create;
using EvrenDev.Application.Catalog.Notes.Queries.Delete;
using EvrenDev.Application.Catalog.Notes.Queries.Get;

namespace EvrenDev.PublicApi.Controllers.Catalog;

[Route("lesson-pages/{lessonPageId:guid}/notes")]
public class NotesController : VersionedApiController
{
    [HttpGet]
    [MustHavePermission(ApiAction.View, ApiResource.Notes)]
    [OpenApiOperation("Get notes for a lesson page.", "")]
    public Task<ApiResponse<List<NoteDto>>> GetByLessonPageAsync(Guid lessonPageId)
    {
        return Mediator.Send(new GetNotesByLessonPageRequest(lessonPageId)).ContinueWith(t =>
            ApiResponse<List<NoteDto>>.Success(t.Result));
    }

    [HttpGet("~/notes/{id:guid}")]
    [MustHavePermission(ApiAction.View, ApiResource.Notes)]
    [OpenApiOperation("Get note details.", "")]
    public async Task<ApiResponse<NoteDto>> GetAsync(Guid id)
    {
        var data = await Mediator.Send(new GetNoteRequest(id));

        if (data == null)
            throw new NotFoundException($"Note with ID '{id}' not found.");

        return ApiResponse<NoteDto>.Success(data);
    }

    [HttpPost]
    [MustHavePermission(ApiAction.Create, ApiResource.Notes)]
    [OpenApiOperation("Create a new note.", "")]
    public Task<Guid> CreateAsync(CreateNoteRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpDelete("~/notes/{id:guid}")]
    [MustHavePermission(ApiAction.Delete, ApiResource.Notes)]
    [OpenApiOperation("Delete a note.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteNoteRequest(id));
    }
}
