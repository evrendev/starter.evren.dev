using EvrenDev.Application.Catalog.Categories.Entities;
using EvrenDev.Application.Catalog.Categories.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Categories.Queries.Get;

public class GetCategoryRequest(Guid id) : IRequest<CategoryDto>
{
    public Guid Id { get; set; } = id;
}

public class GetCategoryRequestHandler(IRepository<Category> repository, IStringLocalizer<GetCategoryRequestHandler> localizer)
    : IRequestHandler<GetCategoryRequest, CategoryDto>
{
    public async Task<CategoryDto> Handle(GetCategoryRequest request, CancellationToken cancellationToken) =>
        await repository.FirstOrDefaultAsync(
            new CategoryByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(localizer["category.notfound"], request.Id));
}
