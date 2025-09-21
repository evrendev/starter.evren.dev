using EvrenDev.Application.Catalog.Categories.Entities;
using EvrenDev.Application.Catalog.Categories.Specifications;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Categories.Queries.Paginate;

public class PaginateCategoriesFilter : PaginationFilter, IRequest<PaginationResponse<CategoryDto>>
{
}

public class PaginateCategoriesFilterHandler
    (IReadRepository<Category> repository) : IRequestHandler<PaginateCategoriesFilter, PaginationResponse<CategoryDto>>
{
    public async Task<PaginationResponse<CategoryDto>> Handle(PaginateCategoriesFilter request,
        CancellationToken cancellationToken)
    {
        var spec = new CategoriesBySearchRequestSpec(request);
        return await repository.PaginatedListAsync(spec, request.Page, request.ItemsPerPage, cancellationToken);
    }
}
