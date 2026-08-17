namespace EvrenDev.Application.Common.Interfaces;

public class PayPalCreateOrderResult
{
    public string PayPalOrderId { get; set; } = default!;
    // HATEOAS "approve" link from the create-order response, if PayPal returns
    // one for this flow — the PayPal JS SDK buttons (client-side) don't need
    // it (they drive approval via the order id directly), but it's returned
    // defensively for any redirect-based fallback (see Task Q0).
    public string? ApproveUrl { get; set; }
    // Resolved from PayPalSettings.Currency on the Infrastructure side — kept
    // out of the Application-layer call so no Application code needs to
    // reference Infrastructure's settings POCO (layering, see CLAUDE.md #1).
    public string Currency { get; set; } = default!;
}

public class PayPalCaptureResult
{
    public bool Succeeded { get; set; }
    public string? PayPalCaptureId { get; set; }
}

public interface IPayPalService : ITransientService
{
    Task<PayPalCreateOrderResult> CreateOrderAsync(Guid courseId, decimal amount, CancellationToken cancellationToken);

    Task<PayPalCaptureResult> CaptureOrderAsync(string payPalOrderId, CancellationToken cancellationToken);

    // headers: the incoming webhook request's HTTP headers (paypal-transmission-id,
    // paypal-transmission-time, paypal-cert-url, paypal-auth-algo, paypal-transmission-sig),
    // rawBody: the exact, unmodified request body bytes as received — PayPal's signature
    // is computed over the raw bytes, so this must not be a re-serialized/reparsed JSON string.
    Task<bool> VerifyWebhookSignatureAsync(IReadOnlyDictionary<string, string> headers, string rawBody,
        CancellationToken cancellationToken);
}
