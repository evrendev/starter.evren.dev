using EvrenDev.Application.Catalog.Absences.Entities;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Absences.Specifications;

public class AbsenceByIdSpec : Specification<Absence, AbsenceDto>, ISingleResultSpecification<Absence>
{
    public AbsenceByIdSpec(Guid id) =>
        Query.Where(p => p.Id == id);
}
