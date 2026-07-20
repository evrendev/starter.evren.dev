using EvrenDev.Application.Catalog.Pages.Entities;
using EvrenDev.Application.Catalog.Pages.Specifications;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Queries.Paginate;

public class PaginatePagesFilter : PaginationFilter, IRequest<PaginationResponse<PageDto>>
{
    public Guid? ChapterId { get; set; }
}

public class PaginatePagesFilterHandler(IReadRepository<Page> repository) : IRequestHandler<PaginatePagesFilter, PaginationResponse<PageDto>>
{
    public async Task<PaginationResponse<PageDto>> Handle(PaginatePagesFilter request, CancellationToken cancellationToken)
    {
        var spec = new PagesBySearchRequestWithChaptersSpec(request);
        return await repository.PaginatedListAsync(spec, request.Page, request.ItemsPerPage, cancellationToken);
    }
}
