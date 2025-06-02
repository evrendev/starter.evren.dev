using EvrenDev.Application.Auditing.Entities;
using EvrenDev.Application.Auditing.Interfaces;

namespace EvrenDev.Application.Auditing.Queries.Get;

public class GetMyAuditLogsRequest : IRequest<List<AuditDto>>
{
}

public class GetMyAuditLogsRequestHandler(ICurrentUser currentUser, IAuditService auditService)
    : IRequestHandler<GetMyAuditLogsRequest, List<AuditDto>>
{
    public Task<List<AuditDto>> Handle(GetMyAuditLogsRequest request, CancellationToken cancellationToken) =>
        auditService.GetUserTrailsAsync(currentUser.GetUserId());
}
