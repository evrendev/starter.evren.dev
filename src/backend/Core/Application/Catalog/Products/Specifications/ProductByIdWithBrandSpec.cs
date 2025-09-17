using EvrenDev.Application.Catalog.Products.Entities;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Products.Specifications;

public class ProductByIdWithBrandSpec : Specification<Product, ProductDetailsDto>, ISingleResultSpecification<Product>
{
    public ProductByIdWithBrandSpec(Guid id)
    {
        Query
            .Where(p => p.Id == id)
            .Include(p => p.Brand);
    }
}
