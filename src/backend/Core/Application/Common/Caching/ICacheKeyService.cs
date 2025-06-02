namespace EvrenDev.Application.Common.Caching;

public interface ICacheKeyService : IScopedService
{
    string GetCacheKey(string name, object id, bool includeTenantId = true);
}
