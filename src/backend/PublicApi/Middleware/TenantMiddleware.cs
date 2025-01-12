namespace EvrenDev.PublicApi.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (tenantId != Guid.Empty || tenantId != null)
        {
            var isValid = await tenantService.SetCurrentTenantAsync(tenantId);
            if (!isValid)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid tenant" });
                return;
            }
        }

        await _next(context);
    }
}
