using EvrenDev.Application.Catalog.Lessons.Entities;
using EvrenDev.Application.Catalog.Lessons.Queries.Create;
using EvrenDev.Application.Catalog.Lessons.Queries.Delete;
using EvrenDev.Application.Catalog.Lessons.Queries.Export;
using EvrenDev.Application.Catalog.Lessons.Queries.Get;
using EvrenDev.Application.Catalog.Lessons.Queries.Paginate;
using EvrenDev.Application.Catalog.Lessons.Queries.Update;

namespace EvrenDev.PublicApi.Controllers.Catalog;

public class LessonsController : VersionedApiController
{
    [HttpGet]
    [MustHavePermission(ApiAction.View, ApiResource.Lessons)]
    [OpenApiOperation("Search lessons using available filters.", "")]
    public Task<PaginationResponse<LessonDto>> GetPaginatatedListAsync([FromQuery] PaginateLessonsFilter request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("all")]
    [MustHavePermission(ApiAction.View, ApiResource.Lessons)]
    [OpenApiOperation("Get all lessons.", "")]
    public async Task<ApiResponse<List<LessonExportDto>?>> GetAllAsync()
    {
        var response = await Mediator.Send(new GetAllLessonsRequest());

        return ApiResponse<List<LessonExportDto>?>.Success(response);
    }

    [HttpGet("{id:guid}")]
    [MustHavePermission(ApiAction.View, ApiResource.Lessons)]
    [OpenApiOperation("Get lesson details.", "")]
    public async Task<ApiResponse<LessonDetailsDto>> GetAsync(Guid id)
    {
        var data = await Mediator.Send(new GetLessonRequest(id));

        if (data == null)
            throw new NotFoundException($"Lesson with ID '{id}' not found.");

        return ApiResponse<LessonDetailsDto>.Success(data);
    }

    [HttpPost]
    [MustHavePermission(ApiAction.Create, ApiResource.Lessons)]
    [OpenApiOperation("Create a new lesson.", "")]
    public async Task<Guid> CreateAsync(CreateLessonRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(ApiAction.Update, ApiResource.Lessons)]
    [OpenApiOperation("Update a lesson.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateLessonRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(ApiAction.Delete, ApiResource.Lessons)]
    [OpenApiOperation("Delete a lesson.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteLessonRequest(id));
    }

    [HttpPost("export")]
    [MustHavePermission(ApiAction.Export, ApiResource.Lessons)]
    [OpenApiOperation("Export a lessons.", "")]
    public async Task<FileResult> ExportAsync(ExportLessonsRequest filter)
    {
        var result = await Mediator.Send(filter);
        return File(result, "application/octet-stream", "LessonExports");
    }
}
