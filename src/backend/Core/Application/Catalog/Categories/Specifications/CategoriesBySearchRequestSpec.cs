using EvrenDev.Application.Catalog.Categories.Entities;
using EvrenDev.Application.Catalog.Categories.Queries.Search;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Categories.Specifications;

public class CategoriesBySearchRequestSpec : EntitiesByPaginationFilterSpec<Category, CategoryDto>
{
    public CategoriesBySearchRequestSpec(PaginateCategoriesFilter request)
        : base(request)
    {
        Query.OrderBy(c => c.Name, !request.HasOrderBy());
    }
}
