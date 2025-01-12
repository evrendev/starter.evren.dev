using EvrenDev.Domain.Entities.Tenant;

namespace EvrenDev.Application.Common.Interfaces;

public interface ITenantService
{
    Guid? GetCurrentTenantId();
    Task<TenantEntity?> GetTenantAsync(Guid? tenantId);
    Task<string?> GetConnectionStringAsync(Guid? tenantId);
    Task<bool> SetCurrentTenantAsync(Guid? tenantId);
}
