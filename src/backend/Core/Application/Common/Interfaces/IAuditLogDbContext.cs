using EvrenDev.Domain.Entities.Audit;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Common.Interfaces;

public interface IAuditLogDbContext
{
    DbSet<AuditLog> Audits { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
