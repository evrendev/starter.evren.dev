using EvrenDev.Application.Catalog.Brands.Entities;
using EvrenDev.Application.Catalog.Brands.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Brands.Queries.Get;

public class GetBrandRequest(Guid id) : IRequest<BrandDto>
{
    public Guid Id { get; set; } = id;
}

public class GetBrandRequestHandler(IRepository<Brand> repository, IStringLocalizer<GetBrandRequestHandler> localizer)
    : IRequestHandler<GetBrandRequest, BrandDto>
{
    public async Task<BrandDto> Handle(GetBrandRequest request, CancellationToken cancellationToken) =>
        await repository.FirstOrDefaultAsync(
            new BrandByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(localizer["brand.notfound"], request.Id));
}
