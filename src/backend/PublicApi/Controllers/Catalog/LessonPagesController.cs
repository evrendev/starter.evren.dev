using EvrenDev.Application.Catalog.LessonPages.Entities;
using EvrenDev.Application.Catalog.LessonPages.Queries.Create;
using EvrenDev.Application.Catalog.LessonPages.Queries.Delete;
using EvrenDev.Application.Catalog.LessonPages.Queries.Export;
using EvrenDev.Application.Catalog.LessonPages.Queries.Get;
using EvrenDev.Application.Catalog.LessonPages.Queries.MarkComplete;
using EvrenDev.Application.Catalog.LessonPages.Queries.Paginate;
using EvrenDev.Application.Catalog.LessonPages.Queries.Update;

namespace EvrenDev.PublicApi.Controllers.Catalog;

public class LessonPagesController : VersionedApiController
{
    [HttpPost]
    [MustHavePermission(ApiAction.Create, ApiResource.LessonPages)]
    [OpenApiOperation("Create a new lesson page.", "")]
    public Task<Guid> CreateAsync(CreateLessonPageRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet]
    [MustHavePermission(ApiAction.View, ApiResource.LessonPages)]
    [OpenApiOperation("Get lesson pages paginated.", "")]
    public Task<PaginationResponse<LessonPageDto>> GetPaginatedListAsync([FromQuery] PaginateLessonPagesFilter request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [MustHavePermission(ApiAction.View, ApiResource.LessonPages)]
    [OpenApiOperation("Get lesson page details.", "")]
    public async Task<ApiResponse<LessonPageDetailsDto>> GetAsync(Guid id)
    {
        var data = await Mediator.Send(new GetLessonPageRequest(id));

        if (data == null)
            throw new NotFoundException($"Lesson page with ID '{id}' not found.");

        return ApiResponse<LessonPageDetailsDto>.Success(data);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(ApiAction.Update, ApiResource.LessonPages)]
    [OpenApiOperation("Update a lesson page.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateLessonPageRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(ApiAction.Delete, ApiResource.LessonPages)]
    [OpenApiOperation("Delete a lesson page.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteLessonPageRequest(id));
    }

    [HttpPost("export")]
    [MustHavePermission(ApiAction.Export, ApiResource.LessonPages)]
    [OpenApiOperation("Export lesson pages.", "")]
    public async Task<FileResult> ExportAsync(ExportLessonPagesRequest filter)
    {
        var result = await Mediator.Send(filter);
        return File(result, "application/octet-stream", "LessonPageExports");
    }

    [HttpGet("{lessonId:guid}/player")]
    [Authorize]
    [OpenApiOperation("Get lesson player view.", "")]
    public async Task<ApiResponse<LessonPlayerDto>> GetPlayerAsync(Guid lessonId)
    {
        var data = await Mediator.Send(new GetLessonPlayerRequest(lessonId));

        return ApiResponse<LessonPlayerDto>.Success(data);
    }

    [HttpPost("{id:guid}/complete")]
    [Authorize]
    [OpenApiOperation("Mark lesson page as completed.", "")]
    public async Task<ApiResponse<bool>> MarkCompletedAsync(Guid id)
    {
        var result = await Mediator.Send(new MarkLessonPageCompletedRequest(id));

        return ApiResponse<bool>.Success(result);
    }
}
