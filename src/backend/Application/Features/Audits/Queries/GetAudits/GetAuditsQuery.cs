using EvrenDev.Application.Features.Audits.Models;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Audits.Queries.GetAudits;

public class GetAuditsQuery : IRequest<Result<List<BasicAuditDto>>>
{
    public string? SearchString { get; set; }
    public string? Action { get; init; }
    public DateTime? StartDate { get; init; } = null;
    public DateTime? EndDate { get; init; } = null;
}

public class GetAuditsQueryHandler : IRequestHandler<GetAuditsQuery, Result<List<BasicAuditDto>>>
{
    private readonly IAuditLogDbContext _context;

    public GetAuditsQueryHandler(IAuditLogDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<BasicAuditDto>>> Handle(GetAuditsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Audits.OrderByDescending(entity => entity.AuditDateTimeUtc).AsQueryable();

        if (request.StartDate != null)
            query = query.Where(entity => entity.AuditDateTimeUtc >= request.StartDate);

        if (request.EndDate != null)
            query = query.Where(entity => entity.AuditDateTimeUtc <= request.EndDate);

        if (!string.IsNullOrEmpty(request.Action))
            query = query.Where(entity => entity.Action == request.Action);

        if (!string.IsNullOrEmpty(request.SearchString))
            query = query.Where(entity =>
                (entity.IpAddress != null && entity.IpAddress.Contains(request.SearchString))
                ||
                (entity.UserId != null && entity.UserId.Contains(request.SearchString))
                ||
                (entity.Email != null && entity.Email.Contains(request.SearchString))
                ||
                (entity.FullName != null && entity.FullName.Contains(request.SearchString))
                ||
                (entity.EntityType != null && entity.EntityType.Contains(request.SearchString))
                ||
                (entity.TablePk != null && entity.TablePk.Contains(request.SearchString))
                ||
                (entity.Action != null && entity.Action.Contains(request.SearchString))
            );

        var auditLogs = await query
            .OrderBy(x => x.AuditDateTimeUtc)
            .Select(x => new BasicAuditDto
            {
                Id = x.Id,
                FullName = x.FullName,
                DateTime = x.AuditDateTimeUtc,
                Action = AuditAction.From(x.Action),
                EntityType = x.EntityType

            })
            .ToListAsync(cancellationToken);

        return Result<List<BasicAuditDto>>.Success(auditLogs);
    }
}
