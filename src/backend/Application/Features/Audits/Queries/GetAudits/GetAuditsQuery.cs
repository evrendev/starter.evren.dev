using System.Reflection;
using EvrenDev.Application.Features.Audits.Models;
using EvrenDev.Domain.Entities.Audit;

namespace EvrenDev.Application.Features.Audits.Queries.GetAudits;

public class GetAuditsQuery : IRequest<Result<PaginatedList<BasicAuditDto>>>
{
    public string? SearchString { get; set; }
    public string? Action { get; init; }
    public DateTime? StartDate { get; init; } = null;
    public DateTime? EndDate { get; init; } = null;
    public int Page { get; init; } = 1;
    public int ItemsPerPage { get; init; } = 10;
    public string? SortBy { get; init; }
    public string SortDesc { get; init; } = "desc";
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

        if (!string.IsNullOrEmpty(request.SortBy) && !string.IsNullOrEmpty(request.SortDesc))
        {
            var propertyInfo = typeof(AuditLog).GetProperty(request.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
             ?? throw new ArgumentException($"Property {request.SortBy} not found", nameof(request.SortBy));

            query = request.SortDesc == "desc"
                ? query.OrderByDescending(x => propertyInfo.GetValue(x))
                : query.OrderBy(x => propertyInfo.GetValue(x));
        }
        else
        {
            query = query.OrderByDescending(entity => entity.AuditDateTimeUtc);
        }

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
            request.ItemsPerPage
        );

        return Result<PaginatedList<BasicAuditDto>>.Success(paginatedList);
    }
}
