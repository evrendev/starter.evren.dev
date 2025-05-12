using EvrenDev.Application.Features.Absences.Models;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Absences.Queries.GetAbsences;

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
                Title = entity.Title,
                StartDate = DateTimeDto.Create.FromUtc(entity.StartDate),
                EndDate = DateTimeDto.Create.FromUtc(entity.EndDate),
                Description = entity.Description,
                Location = entity.Location,
                Employee = entity.Employee,
                Calendar = entity.Calendar
            })
            .ToListAsync(cancellationToken);

        return Result<List<AbsenceDto>>.Success(entities);
    }
}
