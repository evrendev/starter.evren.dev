using EvrenDev.Domain.Entities.Tenant;

namespace EvrenDev.Application.Common.Services;

public interface ITenantService
{
    string GetCurrentTenant();
    string GetCurrentTenantId();
    Task<TenantEntity?> GetTenantAsync(string tenantId);
    Task<string?> GetConnectionStringAsync(string tenantId);
    Task<bool> SetCurrentTenantAsync(string tenantId);
}
