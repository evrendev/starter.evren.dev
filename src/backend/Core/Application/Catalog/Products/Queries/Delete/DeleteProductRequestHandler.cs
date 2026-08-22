using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Products.Queries.Delete;

public class DeleteProductRequest(Guid id) : IRequest<Guid>
{
    public Guid Id { get; set; } = id;
}

public class DeleteProductRequestHandler(IRepositoryWithEvents<Product> repository,
        IStringLocalizer<DeleteProductRequestHandler> localizer)
    : IRequestHandler<DeleteProductRequest, Guid>
{
    public async Task<Guid> Handle(DeleteProductRequest request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = product ?? throw new NotFoundException(localizer["catalog.products.delete.notfound"]);

        // Add Domain Events automatically by using IRepositoryWithEvents
        await repository.DeleteAsync(product, cancellationToken);

        return request.Id;
    }
}
