using EvrenDev.Application.Auditing.Entities;
using EvrenDev.Application.Auditing.Queries.Get;

namespace EvrenDev.Application.Auditing.Interfaces;

public interface IAuditService : ITransientService
{
    Task<PaginationResponse<AuditDto>> PaginatedListAsync(PaginateAuditLogsFilter filter, Guid userId, CancellationToken cancellationToken = default);
}
