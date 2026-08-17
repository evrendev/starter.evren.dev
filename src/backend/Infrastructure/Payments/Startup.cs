using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvrenDev.Infrastructure.Payments;

internal static class Startup
{
    internal static IServiceCollection AddPayments(this IServiceCollection services,
        IConfiguration config)
    {
        // IPayPalService itself is auto-registered via the ITransientService scan
        // (see Infrastructure/Common/Startup.cs) — this only wires the settings
        // binding, same shape as AddMailing/MailSettings.
        return services.Configure<PayPalSettings>(config.GetSection(nameof(PayPalSettings)));
    }
}
