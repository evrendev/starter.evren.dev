using EvrenDev.Application.Features.Audits.Models;
using EvrenDev.Domain.Entities.Audit;

namespace EvrenDev.Application.Features.Audits.Queries.GetAudits;

public class GetAuditsQuery : IRequest<Result<PaginatedList<BasicAuditDto>>>
{
    public string? Search { get; set; }
    public string? Action { get; init; }
    public DateTime? StartDate { get; init; } = null;
    public DateTime? EndDate { get; init; } = null;
    public int Page { get; init; } = 1;
    public int ItemsPerPage { get; init; } = 25;
    public string? SortBy { get; init; }
    public string? SortDesc { get; init; }
}

public class GetAuditsQueryHandler : IRequestHandler<GetAuditsQuery, Result<PaginatedList<BasicAuditDto>>>
{
    private readonly IAuditLogDbContext _context;

    public GetAuditsQueryHandler(IAuditLogDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<BasicAuditDto>>> Handle(GetAuditsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Audits.AsQueryable();

        if (request.StartDate != null)
            query = query.Where(entity => entity.AuditDateTimeUtc >= request.StartDate);

        if (request.EndDate != null)
            query = query.Where(entity => entity.AuditDateTimeUtc <= request.EndDate);

        if (!string.IsNullOrEmpty(request.Action))
            query = query.Where(entity => entity.Action == request.Action);

        if (!string.IsNullOrEmpty(request.Search))
            query = query.Where(entity =>
                (entity.IpAddress != null && entity.IpAddress.Contains(request.Search))
                ||
                (entity.UserId != null && entity.UserId.Contains(request.Search))
                ||
                (entity.Email != null && entity.Email.Contains(request.Search))
                ||
                (entity.FullName != null && entity.FullName.Contains(request.Search))
                ||
                (entity.EntityType != null && entity.EntityType.Contains(request.Search))
                ||
                (entity.TablePk != null && entity.TablePk.Contains(request.Search))
                ||
                (entity.Action != null && entity.Action.Contains(request.Search))
            );

        // Apply sorting
        query = !string.IsNullOrEmpty(request.SortBy) && !string.IsNullOrEmpty(request.SortDesc)
            ? ApplySorting(query, request.SortBy, request.SortDesc == "desc")
            : query.OrderByDescending(x => x.AuditDateTimeUtc);

        var dtoQuery = query.Select(auditLog => new BasicAuditDto
        {
            Id = auditLog.Id,
            Email = auditLog.Email,
            DateTime = DateTimeDto.Create.FromUtc(auditLog.AuditDateTimeUtc),
            Action = AuditAction.From(auditLog.Action),
            EntityType = auditLog.EntityType
        });

        var paginatedList = await PaginatedList<BasicAuditDto>.CreateAsync(
            dtoQuery,
            request.Page,
            request.ItemsPerPage);

        return Result<PaginatedList<BasicAuditDto>>.Success(paginatedList);
    }

    private static IQueryable<AuditLog> ApplySorting(IQueryable<AuditLog> query, string sortBy, bool sortDesc)
    {
        return sortBy.ToLower() switch
        {
            "id" => sortDesc
                ? query.OrderByDescending(x => x.Id)
                : query.OrderBy(x => x.Id),
            "email" => sortDesc
                ? query.OrderByDescending(x => x.Email)
                : query.OrderBy(x => x.Email),
            "auditdatetimeutc" => sortDesc
                ? query.OrderByDescending(x => x.AuditDateTimeUtc)
                : query.OrderBy(x => x.AuditDateTimeUtc),
            "action" => sortDesc
                ? query.OrderByDescending(x => x.Action)
                : query.OrderBy(x => x.Action),
            "entitytype" => sortDesc
                ? query.OrderByDescending(x => x.EntityType)
                : query.OrderBy(x => x.EntityType),
            _ => query.OrderByDescending(x => x.AuditDateTimeUtc) // Default sorting
        };
    }
}
