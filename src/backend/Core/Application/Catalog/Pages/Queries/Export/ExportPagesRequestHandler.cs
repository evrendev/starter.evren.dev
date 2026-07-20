using EvrenDev.Application.Catalog.Pages.Entities;
using EvrenDev.Application.Common.Exporters;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Queries.Export;

public class ExportPagesRequest : BaseFilter, IRequest<Stream>
{
    public Guid? ChapterId { get; set; }
}

public class ExportPagesWithChaptersSpecification : EntitiesByBaseFilterSpec<Page, PageExportDto>
{
    public ExportPagesWithChaptersSpecification(ExportPagesRequest request)
        : base(request) =>
        Query
            .Include(p => p.Chapter)
            .Where(p => p.ChapterId.Equals(request.ChapterId!.Value), request.ChapterId.HasValue)
            .OrderBy(p => p.Order);
}

public class ExportPagesRequestHandler(IReadRepository<Page> repository, IExcelWriter excelWriter)
    : IRequestHandler<ExportPagesRequest, Stream>
{
    public async Task<Stream> Handle(ExportPagesRequest request, CancellationToken cancellationToken)
    {
        var spec = new ExportPagesWithChaptersSpecification(request);

        var list = await repository.ListAsync(spec, cancellationToken);

        return excelWriter.WriteToStream(list);
    }
}
