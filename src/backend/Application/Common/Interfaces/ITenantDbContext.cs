using Microsoft.EntityFrameworkCore;
using EvrenDev.Domain.Entities.Tenant;

namespace EvrenDev.Application.Common.Interfaces;

public interface ITenantDbContext
{
    DbSet<TenantEntity> Tenants { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
