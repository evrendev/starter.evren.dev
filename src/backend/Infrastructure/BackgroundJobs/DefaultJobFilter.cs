using EvrenDev.Infrastructure.Common;
using EvrenDev.Shared.Authorization;
using EvrenDev.Shared.Multitenancy;
using Finbuckle.MultiTenant.Abstractions;
using Hangfire.Client;
using Hangfire.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TenantInfo = EvrenDev.Domain.Multitenancy.TenantInfo;

namespace EvrenDev.Infrastructure.BackgroundJobs;

public class DefaultJobFilter(IServiceProvider services) : IClientFilter
{
    private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

    public void OnCreating(CreatingContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        Logger.InfoFormat("Set TenantId and UserId parameters to job {0}.{1}...",
            context.Job.Method.ReflectedType?.FullName, context.Job.Method.Name);

        using var scope = services.CreateScope();

        var httpContext = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>()?.HttpContext;
        _ = httpContext ?? throw new InvalidOperationException("Can't create a TenantJob without HttpContext.");

        // ITenantInfo is no longer directly DI-resolvable as of Finbuckle v10 — read the
        // concrete TenantInfo through the accessor instead (see Task S3).
        var tenantInfo = scope.ServiceProvider.GetRequiredService<IMultiTenantContextAccessor<TenantInfo>>()
            .MultiTenantContext?.TenantInfo;
        context.SetJobParameter(MultitenancyConstants.TenantIdName, tenantInfo);

        var userId = httpContext.User.GetUserId();
        context.SetJobParameter(QueryStringKeys.UserId, userId);
    }

    public void OnCreated(CreatedContext context)
    {
        Logger.InfoFormat(
            "Job created with parameters {0}",
            context.Parameters.Select(x => x.Key + "=" + x.Value).Aggregate((s1, s2) => s1 + ";" + s2));
    }
}
