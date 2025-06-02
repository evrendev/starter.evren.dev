using EvrenDev.Application.Catalog.Products.Entities;
using EvrenDev.Application.Common.Exporters;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Products.Queries.Export;

public class ExportProductsRequest : BaseFilter, IRequest<Stream>
{
    public Guid? BrandId { get; set; }
    public decimal? MinimumRate { get; set; }
    public decimal? MaximumRate { get; set; }
}

public class ExportProductsWithBrandsSpecification : EntitiesByBaseFilterSpec<Product, ProductExportDto>
{
    public ExportProductsWithBrandsSpecification(ExportProductsRequest request)
        : base(request) =>
        Query
            .Include(p => p.Brand)
            .Where(p => p.BrandId.Equals(request.BrandId!.Value), request.BrandId.HasValue)
            .Where(p => p.Rate >= request.MinimumRate!.Value, request.MinimumRate.HasValue)
            .Where(p => p.Rate <= request.MaximumRate!.Value, request.MaximumRate.HasValue);
}

public class ExportProductsRequestHandler(IReadRepository<Product> repository, IExcelWriter excelWriter)
    : IRequestHandler<ExportProductsRequest, Stream>
{
    public async Task<Stream> Handle(ExportProductsRequest request, CancellationToken cancellationToken)
    {
        var spec = new ExportProductsWithBrandsSpecification(request);

        var list = await repository.ListAsync(spec, cancellationToken);

        return excelWriter.WriteToStream(list);
    }
}
