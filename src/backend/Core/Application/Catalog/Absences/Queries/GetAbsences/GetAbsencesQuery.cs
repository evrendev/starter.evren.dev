using EvrenDev.Application.Catalog.Absences.Models;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Catalog.Absences.Queries.GetAbsences;

public class GetAbsencesQuery : IRequest<Result<List<AbsenceDto>>>
{
    public bool ShowDeletedItems { get; set; } = false;
}

public class GetAbsencesQueryHandler : IRequestHandler<GetAbsencesQuery, Result<List<AbsenceDto>>>
{
    private readonly ICatalogDbContext _context;

    public GetAbsencesQueryHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<AbsenceDto>>> Handle(GetAbsencesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Absences.AsQueryable();

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

        return Result<List<AbsenceDto>>.Success(entities);
    }
}
