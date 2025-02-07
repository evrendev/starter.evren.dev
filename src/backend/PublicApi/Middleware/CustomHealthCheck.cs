using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EvrenDev.PublicApi.Middleware;

public class CustomHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var currentTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        var healthCheckResult = HealthCheckResult.Healthy($"Sağlık kontrolü başarılı. Şu anki saat: {currentTime}");

        return Task.FromResult(healthCheckResult);
    }
}
