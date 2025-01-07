using System.Globalization;
using Microsoft.Extensions.Primitives;

namespace EvrenDev.PublicApi.Middleware;

public class LocalizationMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Headers.TryGetValue("Accept-Language", out StringValues headerCultureKey)
            && DoesCultureExist(headerCultureKey))
        {
            var culture = new CultureInfo(headerCultureKey.ToString());
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        await next(context);
    }

    private static bool DoesCultureExist(string? cultureName)
    {
        return CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Any(culture => string.Equals(culture.Name, cultureName, StringComparison.OrdinalIgnoreCase));
    }
}
