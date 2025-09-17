using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Categories.Specifications;

public class CategoryByNameSpec : Specification<Category>, ISingleResultSpecification<Category>
{
    public CategoryByNameSpec(string name)
    {
        Query.Where(b => b.Name == name);
    }
}
