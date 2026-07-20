using EvrenDev.Application.Catalog.Pages.Entities;
using EvrenDev.Application.Catalog.Pages.Queries.Create;
using EvrenDev.Application.Catalog.Pages.Queries.Delete;
using EvrenDev.Application.Catalog.Pages.Queries.Export;
using EvrenDev.Application.Catalog.Pages.Queries.Get;
using EvrenDev.Application.Catalog.Pages.Queries.Import;
using EvrenDev.Application.Catalog.Pages.Queries.MarkComplete;
using EvrenDev.Application.Catalog.Pages.Queries.Paginate;
using EvrenDev.Application.Catalog.Pages.Queries.Update;

namespace EvrenDev.PublicApi.Controllers.Catalog;

public class PagesController : VersionedApiController
{
    [HttpPost]
    [MustHavePermission(ApiAction.Create, ApiResource.Pages)]
    [OpenApiOperation("Create a new page.", "")]
    public Task<Guid> CreateAsync(CreatePageRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet]
    [MustHavePermission(ApiAction.View, ApiResource.Pages)]
    [OpenApiOperation("Get pages paginated.", "")]
    public Task<PaginationResponse<PageDto>> GetPaginatedListAsync([FromQuery] PaginatePagesFilter request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [MustHavePermission(ApiAction.View, ApiResource.Pages)]
    [OpenApiOperation("Get page details.", "")]
    public async Task<ApiResponse<PageDetailsDto>> GetAsync(Guid id)
    {
        var data = await Mediator.Send(new GetPageRequest(id));

        if (data == null)
            throw new NotFoundException($"Page with ID '{id}' not found.");

        return ApiResponse<PageDetailsDto>.Success(data);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(ApiAction.Update, ApiResource.Pages)]
    [OpenApiOperation("Update a page.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdatePageRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(ApiAction.Delete, ApiResource.Pages)]
    [OpenApiOperation("Delete a page.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeletePageRequest(id));
    }

    [HttpPost("export")]
    [MustHavePermission(ApiAction.Export, ApiResource.Pages)]
    [OpenApiOperation("Export pages.", "")]
    public async Task<FileResult> ExportAsync(ExportPagesRequest filter)
    {
        var result = await Mediator.Send(filter);
        return File(result, "application/octet-stream", "PageExports");
    }

    [HttpGet("{chapterId:guid}/player")]
    [Authorize]
    [OpenApiOperation("Get chapter player view.", "")]
    public async Task<ApiResponse<ChapterPlayerDto>> GetPlayerAsync(Guid chapterId)
    {
        var data = await Mediator.Send(new GetChapterPlayerRequest(chapterId));

        return ApiResponse<ChapterPlayerDto>.Success(data);
    }

    [HttpPost("{id:guid}/complete")]
    [Authorize]
    [OpenApiOperation("Mark page as completed.", "")]
    public async Task<ApiResponse<bool>> MarkCompletedAsync(Guid id)
    {
        var result = await Mediator.Send(new MarkPageCompletedRequest(id));

        return ApiResponse<bool>.Success(result);
    }

    [HttpGet("import/{importJobId:guid}/status")]
    [MustHavePermission(ApiAction.Import, ApiResource.Pages)]
    [OpenApiOperation("Get the progress/status of a PPTX import job.", "")]
    public async Task<ApiResponse<ImportJobDto>> GetImportStatusAsync(Guid importJobId)
    {
        var data = await Mediator.Send(new GetImportJobStatusRequest(importJobId));
        return ApiResponse<ImportJobDto>.Success(data);
    }
}
