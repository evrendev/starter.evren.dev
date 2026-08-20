using EvrenDev.Application.Common.Caching;
using Finbuckle.MultiTenant.Abstractions;
using TenantInfo = EvrenDev.Domain.Multitenancy.TenantInfo;

namespace EvrenDev.Infrastructure.Caching;

// ITenantInfo is no longer directly DI-resolvable as of Finbuckle v10 — read the
// current tenant through IMultiTenantContextAccessor<TenantInfo> instead (Task S3).
public class CacheKeyService(IMultiTenantContextAccessor<TenantInfo> multiTenantContextAccessor) : ICacheKeyService
{
    public string GetCacheKey(string name, object id, bool includeTenantId = true)
    {
        var tenantId = includeTenantId
            ? multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id ??
              throw new InvalidOperationException(
                  "GetCacheKey: includeTenantId set to true and no ITenantInfo available.")
            : "GLOBAL";
        return $"{tenantId}-{name}-{id}";
    }
}
