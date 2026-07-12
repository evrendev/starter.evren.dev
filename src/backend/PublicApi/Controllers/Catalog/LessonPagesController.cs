using EvrenDev.Application.Catalog.LessonPages.Entities;
using EvrenDev.Application.Catalog.LessonPages.Queries.Create;
using EvrenDev.Application.Catalog.LessonPages.Queries.Delete;
using EvrenDev.Application.Catalog.LessonPages.Queries.Get;
using EvrenDev.Application.Catalog.LessonPages.Queries.MarkComplete;
using EvrenDev.Application.Catalog.LessonPages.Queries.Paginate;
using EvrenDev.Application.Catalog.LessonPages.Queries.Update;

namespace EvrenDev.PublicApi.Controllers.Catalog;

[Route("lessons/{lessonId:guid}/pages")]
public class LessonPagesController : VersionedApiController
{
    [HttpPost]
    [MustHavePermission(ApiAction.Create, ApiResource.Lessons)]
    [OpenApiOperation("Create a new lesson page.", "")]
    public async Task<Guid> CreateAsync(Guid lessonId, CreateLessonPageRequest request)
    {
        request.LessonId = lessonId;
        return await Mediator.Send(request);
    }

    [HttpGet]
    [MustHavePermission(ApiAction.View, ApiResource.Lessons)]
    [OpenApiOperation("Get lesson pages paginated.", "")]
    public Task<PaginationResponse<LessonPageDto>> GetPaginatedListAsync(Guid lessonId, [FromQuery] PaginateLessonPagesFilter request)
    {
        request.LessonId = lessonId;
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [Route("~/lesson-pages/{id:guid}")]
    [MustHavePermission(ApiAction.View, ApiResource.Lessons)]
    [OpenApiOperation("Get lesson page details.", "")]
    public async Task<ApiResponse<LessonPageDetailsDto>> GetAsync(Guid id)
    {
        var data = await Mediator.Send(new GetLessonPageRequest(id));

        if (data == null)
            throw new NotFoundException($"Lesson page with ID '{id}' not found.");

        return ApiResponse<LessonPageDetailsDto>.Success(data);
    }

    [HttpPut("~/lesson-pages/{id:guid}")]
    [MustHavePermission(ApiAction.Update, ApiResource.Lessons)]
    [OpenApiOperation("Update a lesson page.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateLessonPageRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("~/lesson-pages/{id:guid}")]
    [MustHavePermission(ApiAction.Delete, ApiResource.Lessons)]
    [OpenApiOperation("Delete a lesson page.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteLessonPageRequest(id));
    }

    [HttpGet("player")]
    [MustHavePermission(ApiAction.View, ApiResource.Lessons)]
    [OpenApiOperation("Get lesson player view.", "")]
    public async Task<ApiResponse<LessonPlayerDto>> GetPlayerAsync(Guid lessonId)
    {
        var data = await Mediator.Send(new GetLessonPlayerRequest(lessonId));

        return ApiResponse<LessonPlayerDto>.Success(data);
    }

    [HttpPost("~/lesson-pages/{id:guid}/complete")]
    [MustHavePermission(ApiAction.Update, ApiResource.Lessons)]
    [OpenApiOperation("Mark lesson page as completed.", "")]
    public async Task<ApiResponse<bool>> MarkCompletedAsync(Guid id)
    {
        var result = await Mediator.Send(new MarkLessonPageCompletedRequest(id));

        return ApiResponse<bool>.Success(result);
    }
}
