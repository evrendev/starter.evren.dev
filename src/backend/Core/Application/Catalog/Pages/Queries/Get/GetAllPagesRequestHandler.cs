using EvrenDev.Application.Catalog.Pages.Entities;
using EvrenDev.Application.Catalog.Pages.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Queries.Get;

public class GetAllPagesRequest() : IRequest<List<PageExportDto>>
{
}

public class GetAllPagesRequestHandler(IRepository<Page> repository, IStringLocalizer<GetAllPagesRequestHandler> localizer)
    : IRequestHandler<GetAllPagesRequest, List<PageExportDto>>
{
    public async Task<List<PageExportDto>> Handle(GetAllPagesRequest request, CancellationToken cancellationToken)
    {
        var pages = await repository.ListAsync(new PagesWithChaptersSpec(), cancellationToken);

        if (pages == null || !pages.Any())
            throw new NotFoundException(string.Format(localizer["catalog.pages.list.notfound"]));

        return pages;
    }
}
