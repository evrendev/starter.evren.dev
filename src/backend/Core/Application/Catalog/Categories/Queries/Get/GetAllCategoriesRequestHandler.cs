using EvrenDev.Application.Catalog.Categories.Entities;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using Mapster;

namespace EvrenDev.Application.Catalog.Categories.Queries.Get;

public class GetAllCategoriesRequest() : IRequest<List<CategoryDto>>
{
}

public class GetAllCategoriesRequestHandler(IRepository<Category> repository, IStringLocalizer<GetAllCategoriesRequestHandler> localizer)
    : IRequestHandler<GetAllCategoriesRequest, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetAllCategoriesRequest request, CancellationToken cancellationToken)
    {
        var categories = await repository.ListAsync();

        if (categories == null || !categories.Any())
            throw new NotFoundException(string.Format(localizer["categories.notfound"]));

        return categories.Adapt<List<CategoryDto>>();
    }
}
