using EvrenDev.Application.Catalog.Chapters.Entities;
using EvrenDev.Application.Catalog.Chapters.Queries.Create;
using EvrenDev.Application.Catalog.Chapters.Queries.Delete;
using EvrenDev.Application.Catalog.Chapters.Queries.Get;
using EvrenDev.Application.Catalog.Chapters.Queries.Paginate;
using EvrenDev.Application.Catalog.Chapters.Queries.Update;

namespace EvrenDev.PublicApi.Controllers.Catalog;

public class ChaptersController : VersionedApiController
{
    [HttpGet]
    [MustHavePermission(ApiAction.View, ApiResource.Chapters)]
    [OpenApiOperation("Get paginated chapters using available filters.", "")]
    public async Task<PaginationResponse<ChapterDto>> GetPaginatatedListAsync([FromQuery] PaginateChaptersFilter filter)
    {
        return await Mediator.Send(filter);
    }

    [HttpGet("all")]
    [MustHavePermission(ApiAction.View, ApiResource.Chapters)]
    [OpenApiOperation("Get all chapters.", "")]
    public async Task<ApiResponse<List<ChapterDto>?>> GetAllAsync()
    {
        var response = await Mediator.Send(new GetAllChaptersRequest());

        return ApiResponse<List<ChapterDto>?>.Success(response);
    }

    [HttpGet("{id:guid}")]
    [MustHavePermission(ApiAction.View, ApiResource.Chapters)]
    [OpenApiOperation("Get chapter details.", "")]
    public async Task<ApiResponse<ChapterDto>> GetAsync(Guid id)
    {
        var data = await Mediator.Send(new GetChapterRequest(id));
        if (data == null)
            throw new NotFoundException($"Chapter with ID '{id}' not found.");

        return ApiResponse<ChapterDto>.Success(data);
    }

    [HttpPost]
    [MustHavePermission(ApiAction.Create, ApiResource.Chapters)]
    [OpenApiOperation("Create a new chapter.", "")]
    public async Task<ApiResponse<Guid>> CreateAsync(CreateChapterRequest request)
    {
        var response = await Mediator.Send(request);

        return ApiResponse<Guid>.Success(response);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(ApiAction.Update, ApiResource.Chapters)]
    [OpenApiOperation("Update a chapter.", "")]
    public async Task<ApiResponse<Guid>> UpdateAsync(UpdateChapterRequest request, Guid id)
    {
        return id != request.Id
            ? ApiResponse<Guid>.Failure("Mismatched Chapter ID")
            : ApiResponse<Guid>.Success(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(ApiAction.Delete, ApiResource.Chapters)]
    [OpenApiOperation("Delete a chapter.", "")]
    public async Task<ApiResponse<Guid>> DeleteAsync(Guid id)
    {
        var response = await Mediator.Send(new DeleteChapterRequest(id));
        return ApiResponse<Guid>.Success(response);
    }
}
