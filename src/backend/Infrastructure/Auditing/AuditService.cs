using EvrenDev.Application.Auditing.Entities;
using EvrenDev.Application.Auditing.Interfaces;
using EvrenDev.Application.Auditing.Queries.Get;
using EvrenDev.Application.Common.Models;

namespace EvrenDev.Infrastructure.Auditing;

public class AuditService(ApplicationDbContext auditDbContext) : IAuditService
{
    public async Task<PaginationResponse<AuditDto>> PaginatedListAsync(PaginateAuditLogsFilter filter, DefaultIdType userId, CancellationToken cancellationToken = default)
    {
        var query = auditDbContext.AuditTrails
            .Where(a => a.UserId == userId)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.Search))
        {
            var searchLower = filter.Search.ToLower();
            query = query.Where(audit =>
                audit.TableName != null && audit.TableName.ToLower().Contains(searchLower) ||
                audit.NewValues != null && audit.NewValues.ToLower().Contains(searchLower)
            );
        }

        if (filter.StartDate.HasValue)
            query = query.Where(t => t.DateTime >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(t => t.DateTime <= filter.EndDate.Value);

        if (filter.SortBy is { Count: > 0 })
        {
            var sortItem = filter.SortBy[0];

            var isDescending = sortItem.Order?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;
            var sortField = sortItem.Key ?? string.Empty;

            switch (sortField.ToLower())
            {
                case "id":
                    query = isDescending ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id);
                    break;
                case "datetime":
                    query = isDescending ? query.OrderByDescending(t => t.DateTime) : query.OrderBy(t => t.DateTime);
                    break;
                default:
                    query = query.OrderBy(t => t.DateTime);
                    break;
            }
        }
        else
        {
            query = query.OrderBy(t => t.Id);
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var pagedData = await query
            .Skip((filter.Page - 1) * filter.ItemsPerPage)
            .Take(filter.ItemsPerPage)
            .ToListAsync(cancellationToken);

        var pagedDataDto = pagedData.Adapt<List<AuditDto>>();

        return new PaginationResponse<AuditDto>(pagedDataDto, totalItems, filter.Page, filter.ItemsPerPage);
    }
}
