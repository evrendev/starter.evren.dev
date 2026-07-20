using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Queries.Delete;

public class DeletePageRequest(Guid id) : IRequest<Guid>
{
    public Guid Id { get; set; } = id;
}

public class DeletePageRequestHandler(IRepository<Page> repository, IStringLocalizer<DeletePageRequestHandler> localizer)
    : IRequestHandler<DeletePageRequest, Guid>
{
    public async Task<Guid> Handle(DeletePageRequest request, CancellationToken cancellationToken)
    {
        var page = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = page ?? throw new NotFoundException(localizer["catalog.pages.delete.notfound"]);

        await repository.DeleteAsync(page, cancellationToken);

        return request.Id;
    }
}
