using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Categories.Specifications;

public class CategoryByTitleSpec : Specification<Category>, ISingleResultSpecification<Category>
{
    public CategoryByTitleSpec(string title)
    {
        Query.Where(b => b.Title == title);
    }
}
