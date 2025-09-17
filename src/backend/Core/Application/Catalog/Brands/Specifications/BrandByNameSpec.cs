using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Brands.Specifications;

public class BrandByNameSpec : Specification<Brand>, ISingleResultSpecification<Brand>
{
    public BrandByNameSpec(string name)
    {
        Query.Where(b => b.Name == name);
    }
}
