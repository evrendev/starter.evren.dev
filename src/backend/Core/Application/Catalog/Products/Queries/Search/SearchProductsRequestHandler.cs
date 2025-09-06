using EvrenDev.Application.Catalog.Products.Entities;
using EvrenDev.Application.Catalog.Products.Specifications;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Products.Queries.Search;

public class SearchProductsRequest : PaginationFilter, IRequest<PaginationResponse<ProductDto>>
{
    public Guid? BrandId { get; set; }
    public decimal? MinimumRate { get; set; }
    public decimal? MaximumRate { get; set; }
}

public class SearchProductsRequestHandler(IReadRepository<Product> repository)
    : IRequestHandler<SearchProductsRequest, PaginationResponse<ProductDto>>
{
    public async Task<PaginationResponse<ProductDto>> Handle(SearchProductsRequest request, CancellationToken cancellationToken)
    {
        var spec = new ProductsBySearchRequestWithBrandsSpec(request);
        return await repository.PaginatedListAsync(spec, request.Page, request.ItemsPerPage, cancellationToken: cancellationToken);
    }
}
