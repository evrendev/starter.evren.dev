using EvrenDev.Application.Catalog.Products.Entities;
using EvrenDev.Application.Catalog.Products.Queries.Create;
using EvrenDev.Application.Catalog.Products.Queries.Delete;
using EvrenDev.Application.Catalog.Products.Queries.Export;
using EvrenDev.Application.Catalog.Products.Queries.Get;
using EvrenDev.Application.Catalog.Products.Queries.Search;
using EvrenDev.Application.Catalog.Products.Queries.Update;

namespace EvrenDev.Host.Controllers.Catalog;

public class ProductsController : VersionedApiController
{
    [HttpPost("search")]
    [MustHavePermission(ApiAction.Search, ApiResource.Products)]
    [OpenApiOperation("Search products using available filters.", "")]
    public Task<PaginationResponse<ProductDto>> SearchAsync(SearchProductsRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [MustHavePermission(ApiAction.View, ApiResource.Products)]
    [OpenApiOperation("Get product details.", "")]
    public Task<ProductDetailsDto> GetAsync(Guid id)
    {
        return Mediator.Send(new GetProductRequest(id));
    }

    [HttpPost]
    [MustHavePermission(ApiAction.Create, ApiResource.Products)]
    [OpenApiOperation("Create a new product.", "")]
    public Task<Guid> CreateAsync(CreateProductRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(ApiAction.Update, ApiResource.Products)]
    [OpenApiOperation("Update a product.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateProductRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(ApiAction.Delete, ApiResource.Products)]
    [OpenApiOperation("Delete a product.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteProductRequest(id));
    }

    [HttpPost("export")]
    [MustHavePermission(ApiAction.Export, ApiResource.Products)]
    [OpenApiOperation("Export a products.", "")]
    public async Task<FileResult> ExportAsync(ExportProductsRequest filter)
    {
        var result = await Mediator.Send(filter);
        return File(result, "application/octet-stream", "ProductExports");
    }
}
