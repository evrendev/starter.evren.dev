using EvrenDev.Application.Catalog.Products.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Brands.Queries.Delete;

public class DeleteBrandRequest(Guid id) : IRequest<Guid>
{
    public Guid Id { get; set; } = id;
}

public class DeleteBrandRequestHandler(
        IRepositoryWithEvents<Brand> brandRepo,
        IReadRepository<Product> productRepo,
        IStringLocalizer<DeleteBrandRequestHandler> localizer)
    : IRequestHandler<DeleteBrandRequest, Guid>
{
    // Add Domain Events automatically by using IRepositoryWithEvents

    public async Task<Guid> Handle(DeleteBrandRequest request, CancellationToken cancellationToken)
    {
        if (await productRepo.AnyAsync(new ProductsByBrandSpec(request.Id), cancellationToken))
            throw new ConflictException(localizer["catalog.brands.delete.cannotbedeleted"]);

        var brand = await brandRepo.GetByIdAsync(request.Id, cancellationToken);

        _ = brand ?? throw new NotFoundException(localizer["catalog.brands.delete.notfound"]);

        await brandRepo.DeleteAsync(brand, cancellationToken);

        return request.Id;
    }
}
