using EvrenDev.Application.Auditing.Entities;
using EvrenDev.Application.Auditing.Interfaces;

namespace EvrenDev.Application.Auditing.Queries.Get;

public class PaginateAuditLogsFilter : PaginationFilter, IRequest<PaginationResponse<AuditDto>>
{
    public string? Type { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class PaginateAuditLogsFilterHandler(ICurrentUser currentUser, IAuditService auditService)
    : IRequestHandler<PaginateAuditLogsFilter, PaginationResponse<AuditDto>>
{
    public Task<PaginationResponse<AuditDto>> Handle(PaginateAuditLogsFilter filter, CancellationToken cancellationToken)
    {
        return auditService.PaginatedListAsync(filter, currentUser.GetUserId(), cancellationToken);
    }
}
