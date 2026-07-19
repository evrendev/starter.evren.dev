using EvrenDev.Application.Catalog.Lessons.Entities;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Lessons.Specifications;

public class ImportJobByIdSpec : Specification<ImportJob, ImportJobDto>, ISingleResultSpecification<ImportJob>
{
    public ImportJobByIdSpec(Guid id) =>
        Query.Where(p => p.Id == id);
}
