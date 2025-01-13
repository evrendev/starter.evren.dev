using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace EvrenDev.PublicApi.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    private readonly IStringLocalizer<TenantMiddleware> _localizer;

    public TenantMiddleware(RequestDelegate next,
        IStringLocalizer<TenantMiddleware> localizer)
    {
        _next = next;
        _localizer = localizer;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService, ILogger<TenantMiddleware> logger)
    {
        // Skip tenant validation for authentication endpoints
        if (context.Request.Path.StartsWithSegments("/api/auth", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var tenantId = tenantService.GetCurrentTenantId();
        logger.LogInformation("Current tenant ID from claims: {TenantId}", tenantId);

        if (tenantId.HasValue)
        {
            var isValid = await tenantService.SetCurrentTenantAsync(tenantId);
            if (!isValid)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { error = _localizer["api.tenants.not-found"].Value });
                return;
            }
        }
        else
        {
            logger.LogWarning("No tenant ID found in claims");
        }

        await _next(context);
    }
}
