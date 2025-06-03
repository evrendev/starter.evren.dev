using EvrenDev.Application.Catalog.Absences.Entities;
using EvrenDev.Application.Catalog.Absences.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Absences.Queries.Get;

public class GetAbsencesQuery(Guid id) : IRequest<AbsenceDto>
{
    public Guid Id { get; set; } = id;
}

public class GetAbsencesQueryHandler(IRepository<Absence> repository, IStringLocalizer<GetAbsencesQueryHandler> localizer) : IRequestHandler<GetAbsencesQuery, AbsenceDto>
{
    public async Task<AbsenceDto> Handle(GetAbsencesQuery request, CancellationToken cancellationToken) =>
        await repository.FirstOrDefaultAsync(
            new AbsenceByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(localizer["absence.notfound"], request.Id));
}
