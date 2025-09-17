using EvrenDev.Application.Catalog.Products.Entities;
using EvrenDev.Application.Catalog.Products.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Products.Queries.Get;

public class GetProductRequest(Guid id) : IRequest<ProductDetailsDto>
{
    public Guid Id { get; set; } = id;
}

public class GetProductRequestHandler(IRepository<Product> repository,
        IStringLocalizer<GetProductRequestHandler> localizer)
    : IRequestHandler<GetProductRequest, ProductDetailsDto>
{
    public async Task<ProductDetailsDto> Handle(GetProductRequest request, CancellationToken cancellationToken)
    {
        return await repository.FirstOrDefaultAsync(
                   new ProductByIdWithBrandSpec(request.Id), cancellationToken)
               ?? throw new NotFoundException(string.Format(localizer["catalog.products.get.notfound"], request.Id));
    }
}
