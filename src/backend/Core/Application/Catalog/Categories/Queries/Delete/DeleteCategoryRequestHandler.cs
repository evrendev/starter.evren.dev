using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Categories.Queries.Delete;

public class DeleteCategoryRequest(Guid id) : IRequest<Guid>
{
    public Guid Id { get; set; } = id;
}

public class DeleteCategoryRequestHandler(
        IRepositoryWithEvents<Category> categoryRepo,
        // IReadRepository<Course> courseRepo,
        IStringLocalizer<DeleteCategoryRequestHandler> localizer)
    : IRequestHandler<DeleteCategoryRequest, Guid>
{
    public async Task<Guid> Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await categoryRepo.GetByIdAsync(request.Id, cancellationToken);

        _ = category ?? throw new NotFoundException(localizer["catalog.categories.delete.notfound"]);

        await categoryRepo.DeleteAsync(category, cancellationToken);

        return request.Id;
    }
}
