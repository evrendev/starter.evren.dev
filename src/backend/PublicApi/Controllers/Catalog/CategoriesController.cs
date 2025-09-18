using EvrenDev.Application.Catalog.Categories.Entities;
using EvrenDev.Application.Catalog.Categories.Queries.Create;
using EvrenDev.Application.Catalog.Categories.Queries.Delete;
using EvrenDev.Application.Catalog.Categories.Queries.Get;
using EvrenDev.Application.Catalog.Categories.Queries.Paginate;
using EvrenDev.Application.Catalog.Categories.Queries.Update;

namespace EvrenDev.PublicApi.Controllers.Catalog;

public class CategoriesController : VersionedApiController
{
    [HttpGet]
    [MustHavePermission(ApiAction.View, ApiResource.Categories)]
    [OpenApiOperation("Get paginated categories using available filters.", "")]
    public async Task<PaginationResponse<CategoryDto>> GetPaginatatedListAsync([FromQuery] PaginateCategoriesFilter filter)
    {
        return await Mediator.Send(filter);
    }

    [HttpGet("all")]
    [MustHavePermission(ApiAction.View, ApiResource.Categories)]
    [OpenApiOperation("Get all categories.", "")]
    public async Task<ApiResponse<List<CategoryDto>?>> GetAllAsync(GetAllCategoriesRequest request)
    {
        var response = await Mediator.Send(request);

        return ApiResponse<List<CategoryDto>?>.Success(response);
    }

    [HttpGet("{id:guid}")]
    [MustHavePermission(ApiAction.View, ApiResource.Categories)]
    [OpenApiOperation("Get category details.", "")]
    public async Task<ApiResponse<CategoryDto>> GetAsync(Guid id)
    {
        var data = await Mediator.Send(new GetCategoryRequest(id));
        if (data == null)
            throw new NotFoundException($"Category with ID '{id}' not found.");

        return ApiResponse<CategoryDto>.Success(data);
    }

    [HttpPost]
    [MustHavePermission(ApiAction.Create, ApiResource.Categories)]
    [OpenApiOperation("Create a new category.", "")]
    public async Task<ApiResponse<Guid>> CreateAsync(CreateCategoryRequest request)
    {
        var response = await Mediator.Send(request);

        return ApiResponse<Guid>.Success(response);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(ApiAction.Update, ApiResource.Categories)]
    [OpenApiOperation("Update a category.", "")]
    public async Task<ApiResponse<Guid>> UpdateAsync(UpdateCategoryRequest request, Guid id)
    {
        return id != request.Id
            ? ApiResponse<Guid>.Failure("Mismatched Category ID")
            : ApiResponse<Guid>.Success(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(ApiAction.Delete, ApiResource.Categories)]
    [OpenApiOperation("Delete a category.", "")]
    public async Task<ApiResponse<Guid>> DeleteAsync(Guid id)
    {
        var response = await Mediator.Send(new DeleteCategoryRequest(id));
        return ApiResponse<Guid>.Success(response);
    }
}
