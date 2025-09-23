using EvrenDev.Application.Catalog.Categories.Entities;
using EvrenDev.Application.Catalog.Categories.Queries.Paginate;
using EvrenDev.Application.Common.Specification;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Categories.Specifications;

public class CategoriesBySearchRequestSpec : Specification<Category, CategoryDto>
{
    public CategoriesBySearchRequestSpec(PaginateCategoriesFilter request)
    {
        Query.Where(category =>
                string.IsNullOrEmpty(request.Search)
                ||
                category.Title.ToLower().Contains(request.Search.ToLower())
                ||
                (
                    category.Description != null
                    &&
                    category.Description.ToLower().Contains(request.Search.ToLower())
                )
            )
            .OrderBy(c => c.Title, !request.HasOrderBy());
    }
}
