using EvrenDev.Domain.Entities.Tenant;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Common.Interfaces;

public interface ITenantDbContext
{
    DbSet<TenantEntity> Tenants { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
