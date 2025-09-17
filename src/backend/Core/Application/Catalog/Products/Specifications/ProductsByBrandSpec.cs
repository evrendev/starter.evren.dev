using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Products.Specifications;

public class ProductsByBrandSpec : Specification<Product>
{
    public ProductsByBrandSpec(Guid brandId)
    {
        Query.Where(p => p.BrandId == brandId);
    }
}
