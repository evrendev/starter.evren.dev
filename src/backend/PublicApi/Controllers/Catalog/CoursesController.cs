using EvrenDev.Application.Catalog.Courses.Entities;
using EvrenDev.Application.Catalog.Courses.Queries.Create;
using EvrenDev.Application.Catalog.Courses.Queries.Delete;
using EvrenDev.Application.Catalog.Courses.Queries.Export;
using EvrenDev.Application.Catalog.Courses.Queries.Get;
using EvrenDev.Application.Catalog.Courses.Queries.Paginate;
using EvrenDev.Application.Catalog.Courses.Queries.Update;
using EvrenDev.Application.Catalog.Lessons.Queries.Import;
using Microsoft.AspNetCore.Http;

namespace EvrenDev.PublicApi.Controllers.Catalog;

public class CoursesController : VersionedApiController
{
    [HttpGet]
    [MustHavePermission(ApiAction.View, ApiResource.Courses)]
    [OpenApiOperation("Search courses using available filters.", "")]
    public Task<PaginationResponse<CourseDto>> GetPaginatatedListAsync([FromQuery] PaginateCoursesFilter request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("all")]
    [MustHavePermission(ApiAction.View, ApiResource.Courses)]
    [OpenApiOperation("Get all courses.", "")]
    public async Task<ApiResponse<List<CourseExportDto>?>> GetAllAsync()
    {
        var response = await Mediator.Send(new GetAllCoursesRequest());

        return ApiResponse<List<CourseExportDto>?>.Success(response);
    }

    [HttpGet("{id:guid}")]
    [MustHavePermission(ApiAction.View, ApiResource.Courses)]
    [OpenApiOperation("Get course details.", "")]
    public async Task<ApiResponse<CourseDetailsDto>> GetAsync(Guid id)
    {
        var data = await Mediator.Send(new GetCourseRequest(id));

        if (data == null)
            throw new NotFoundException($"Course with ID '{id}' not found.");

        return ApiResponse<CourseDetailsDto>.Success(data);
    }

    [HttpPost]
    [MustHavePermission(ApiAction.Create, ApiResource.Courses)]
    [OpenApiOperation("Create a new course.", "")]
    public async Task<Guid> CreateAsync(CreateCourseRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(ApiAction.Update, ApiResource.Courses)]
    [OpenApiOperation("Update a course.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateCourseRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(ApiAction.Delete, ApiResource.Courses)]
    [OpenApiOperation("Delete a course.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteCourseRequest(id));
    }

    [HttpPost("export")]
    [MustHavePermission(ApiAction.Export, ApiResource.Courses)]
    [OpenApiOperation("Export a courses.", "")]
    public async Task<FileResult> ExportAsync(ExportCoursesRequest filter)
    {
        var result = await Mediator.Send(filter);
        return File(result, "application/octet-stream", "CourseExports");
    }

    [HttpPost("{id:guid}/import-pptx")]
    [MustHavePermission(ApiAction.Import, ApiResource.Lessons)]
    // Action-scoped override, not a global Kestrel change: the ASP.NET Core default
    // (~28.6MB) would reject any real .pptx well before the request handler even runs.
    // 200MB gives multipart overhead headroom above the 150MB file-size check enforced
    // in ImportLessonsFromPptxRequestHandler.
    [RequestSizeLimit(209_715_200)]
    [OpenApiOperation("Import lessons from a PowerPoint (.pptx) file into a course.", "")]
    public async Task<ActionResult<Guid>> ImportPptxAsync(Guid id, [FromForm] IFormFile file,
        [FromForm] string? slidesHtml = null)
    {
        var importJobId = await Mediator.Send(new ImportLessonsFromPptxRequest
        {
            CourseId = id,
            File = file,
            SlidesHtmlJson = slidesHtml,
        });
        return Ok(importJobId);
    }
}
