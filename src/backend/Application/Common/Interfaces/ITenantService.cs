using EvrenDev.Domain.Entities.Tenant;

namespace EvrenDev.Application.Common.Interfaces;

public interface ITenantService
{
    string GetCurrentTenantId();
    Task<TenantEntity?> GetTenantAsync(string tenantId);
    Task<string?> GetConnectionStringAsync(string tenantId);
    Task<bool> SetCurrentTenantAsync(string tenantId);
}
