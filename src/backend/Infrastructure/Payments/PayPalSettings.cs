namespace EvrenDev.Infrastructure.Payments;

public class PayPalSettings
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }

    // "sandbox" or "live" — selects both the PayPalServerSDK Environment and
    // which REST base URL (api-m.sandbox.paypal.com vs api-m.paypal.com) the
    // raw webhook-verification HttpClient call targets.
    public string Mode { get; set; } = "sandbox";

    // PayPal's own identifier for the webhook subscription configured in the
    // Developer Dashboard — required by /v1/notifications/verify-webhook-signature.
    public string? WebhookId { get; set; }

    // Single global currency (ISO 4217) for all paid courses — see Task Q0:
    // Course has no per-course Currency field and nothing else in this codebase
    // handles multi-currency (admin UI hardcodes "€"), so a per-tenant/global
    // config value is the right scope until a real multi-currency need appears.
    public string Currency { get; set; } = "EUR";
}
