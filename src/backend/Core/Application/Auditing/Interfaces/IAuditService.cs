using EvrenDev.Application.Auditing.Entities;

namespace EvrenDev.Application.Auditing.Interfaces;

public interface IAuditService : ITransientService
{
    Task<List<AuditDto>> GetUserTrailsAsync(Guid userId);
}
