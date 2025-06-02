using EvrenDev.Application.Catalog.Brands.Entities;
using EvrenDev.Application.Catalog.Brands.Specifications;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Brands.Queries.Search;

public class SearchBrandsRequest : PaginationFilter, IRequest<PaginationResponse<BrandDto>>
{
}

public class SearchBrandsRequestHandler(IReadRepository<Brand> repository) : IRequestHandler<SearchBrandsRequest, PaginationResponse<BrandDto>>
{
    public async Task<PaginationResponse<BrandDto>> Handle(SearchBrandsRequest request, CancellationToken cancellationToken)
    {
        var spec = new BrandsBySearchRequestSpec(request);
        return await repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
    }
}
