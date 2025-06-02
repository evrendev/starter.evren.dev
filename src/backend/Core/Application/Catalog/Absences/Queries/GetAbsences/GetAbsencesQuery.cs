using EvrenDev.Application.Catalog.Absences.Models;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Absences.Queries.GetAbsences;

public class GetAbsencesQuery : IRequest<List<AbsenceDto>>
{
}

public class GetAbsencesQueryHandler(IReadRepository<Absence> repository) : IRequestHandler<GetAbsencesQuery, List<AbsenceDto>>
{
    public async Task<List<AbsenceDto>> Handle(GetAbsencesQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository
            .ListAsync(cancellationToken);

        var response = entities.Select(entity => new AbsenceDto
        {
            Id = entity.Id,
            Start = entity.StartDate.ToString("yyyy-MM-dd"),
            End = entity.EndDate.ToString("yyyy-MM-dd"),
            Description = entity.Description,
            Location = entity.Location,
            Employee = entity.Employee,
            CalendarId = entity.CalendarId
        })
        .ToList();

        return response;
    }
}
