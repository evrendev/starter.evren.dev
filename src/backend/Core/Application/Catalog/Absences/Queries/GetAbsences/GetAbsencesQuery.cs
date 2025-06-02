using EvrenDev.Application.Catalog.Absences.Models;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Catalog.Absences.Queries.GetAbsences;

public class GetAbsencesQuery : IRequest<List<AbsenceDto>>
{
}

public class GetAbsencesQueryHandler(IReadRepository<Absence> repository) : IRequestHandler<GetAbsencesQuery, List<AbsenceDto>>
{
    public async Task<List<AbsenceDto>> Handle(GetAbsencesQuery request, CancellationToken cancellationToken)
    {
        var query = repository.AsQueryable();

        if (request.ShowDeletedItems)
            query = query.IgnoreQueryFilters();

        var entities = await query
            .AsNoTracking()
            .Select(entity => new AbsenceDto
            {
                Id = entity.Id,
                Start = entity.StartDate.ToString("yyyy-MM-dd"),
                End = entity.EndDate.ToString("yyyy-MM-dd"),
                Description = entity.Description,
                Location = entity.Location,
                Employee = entity.Employee,
                CalendarId = entity.CalendarId
            })
            .ToListAsync(cancellationToken);

        return entities;
    }
}
