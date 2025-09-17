using EvrenDev.Application.Catalog.Categories.Entities;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Categories.Specifications;

public class CategoryByIdSpec : Specification<Category, CategoryDto>, ISingleResultSpecification<Category>
{
    public CategoryByIdSpec(Guid id)
    {
        Query.Where(p => p.Id == id);
    }
}
