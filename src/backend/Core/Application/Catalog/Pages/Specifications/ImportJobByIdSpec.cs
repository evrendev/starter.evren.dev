using EvrenDev.Application.Catalog.Pages.Entities;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Pages.Specifications;

public class ImportJobByIdSpec : Specification<ImportJob, ImportJobDto>, ISingleResultSpecification<ImportJob>
{
    public ImportJobByIdSpec(Guid id) =>
        Query.Where(p => p.Id == id);
}
