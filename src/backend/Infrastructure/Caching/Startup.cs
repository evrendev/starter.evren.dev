using EvrenDev.Application.Common.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace EvrenDev.Infrastructure.Caching;

internal static class Startup
{
    internal static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration config)
    {
        var settings = config.GetSection(nameof(CacheSettings)).Get<CacheSettings>();
        if (settings!.UseDistributedCache)
        {
            if (settings.PreferRedis)
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = settings.RedisUrl;
                    options.ConfigurationOptions = new ConfigurationOptions
                    {
                        AbortOnConnectFail = true, EndPoints = { settings.RedisUrl }
                    };
                });
            else
                services.AddDistributedMemoryCache();

            services.AddTransient<ICacheService, DistributedCacheService>();
        }
        else
        {
            services.AddMemoryCache();
            services.AddTransient<ICacheService, LocalCacheService>();
        }

        return services;
    }
}
