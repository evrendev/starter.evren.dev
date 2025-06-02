using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Common.Events;
using Microsoft.Extensions.Localization;

namespace EvrenDev.Application.Catalog.Products.Queries.Delete;

public class DeleteProductRequest(Guid id) : IRequest<Guid>
{
    public Guid Id { get; set; } = id;
}

public class DeleteProductRequestHandler(IRepository<Product> repository, IStringLocalizer<DeleteProductRequestHandler> localizer)
    : IRequestHandler<DeleteProductRequest, Guid>
{
    public async Task<Guid> Handle(DeleteProductRequest request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = product ?? throw new NotFoundException(localizer["product.notfound"]);

        // Add Domain Events to be raised after the commit
        product.DomainEvents.Add(EntityDeletedEvent.WithEntity(product));

        await repository.DeleteAsync(product, cancellationToken);

        return request.Id;
    }
}
