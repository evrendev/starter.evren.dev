using EvrenDev.Application.Common.Caching;
using Microsoft.Extensions.Localization;

namespace EvrenDev.Infrastructure.Localization;

public class JsonStringLocalizerFactory(ICacheService cache) : IStringLocalizerFactory
{
    public IStringLocalizer Create(Type resourceSource)
    {
        return new JsonStringLocalizer(cache);
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        return new JsonStringLocalizer(cache);
    }
}
