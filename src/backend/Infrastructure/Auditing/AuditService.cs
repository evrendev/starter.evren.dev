using EvrenDev.Application.Auditing.Entities;
using EvrenDev.Application.Auditing.Interfaces;

namespace EvrenDev.Infrastructure.Auditing;

public class AuditService(ApplicationDbContext context) : IAuditService
{
    public async Task<List<AuditDto>> GetUserTrailsAsync(Guid userId)
    {
        var trails = await context.AuditTrails
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.DateTime)
            .Take(250)
            .ToListAsync();

        return trails.Adapt<List<AuditDto>>();
    }
}
