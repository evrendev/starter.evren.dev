using EvrenDev.Application.Catalog.Brands.Entities;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Brands.Specifications;

public class BrandByIdSpec : Specification<Brand, BrandDto>, ISingleResultSpecification<Brand>
{
    public BrandByIdSpec(Guid id)
    {
        Query.Where(p => p.Id == id);
    }
}
