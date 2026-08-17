using System.ComponentModel;

namespace EvrenDev.Application.Payments.Interfaces;

public interface IProcessPayPalWebhookJob : IScopedService
{
    [DisplayName("Verify and process a PayPal webhook event")]
    Task ExecuteAsync(string rawBody, Dictionary<string, string> headers, CancellationToken cancellationToken);
}
