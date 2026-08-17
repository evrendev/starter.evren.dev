using EvrenDev.Application.Payments.Queries.Capture;
using EvrenDev.Application.Payments.Queries.Create;
using EvrenDev.Application.Payments.Queries.Webhook;

namespace EvrenDev.PublicApi.Controllers.Payments;

public class PaymentsController : VersionedApiController
{
    [HttpPost("orders")]
    [Authorize]
    [OpenApiOperation("Create a PayPal order for a paid course (does not enroll yet).", "")]
    public Task<CreatePaymentOrderResponse> CreateOrderAsync(CreatePaymentOrderRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("orders/{id}/capture")]
    [Authorize]
    [OpenApiOperation("Capture a previously-created PayPal order and enroll the current user.", "")]
    public Task<bool> CaptureOrderAsync(Guid id)
    {
        return Mediator.Send(new CapturePaymentOrderRequest(id));
    }

    // No [Authorize] here — PayPal's webhook caller never authenticates as an
    // application user and sends no bearer token or tenant header; the ENTIRE
    // security boundary for this endpoint is the webhook signature check that
    // happens inside ProcessPayPalWebhookRequestHandler's background job (see
    // ProcessPayPalWebhookJob.VerifyWebhookSignatureAsync). Anything that fails
    // that check is silently dropped, never trusted. Do not add [Authorize] —
    // PayPal cannot supply the credentials it would require.
    [HttpPost("webhooks/paypal")]
    [AllowAnonymous]
    [OpenApiOperation("Receive a PayPal webhook event (signature-verified asynchronously).", "")]
    public async Task<IActionResult> HandlePayPalWebhookAsync()
    {
        // Read the raw, unmodified body — PayPal's signature is computed over
        // these exact bytes, so nothing here may re-serialize or reparse it
        // before it reaches the verification call (see IPayPalService docs).
        using var reader = new StreamReader(Request.Body);
        var rawBody = await reader.ReadToEndAsync();

        var headers = Request.Headers.ToDictionary(h => h.Key.ToLowerInvariant(), h => h.Value.ToString());

        await Mediator.Send(new ProcessPayPalWebhookRequest(rawBody, headers));

        // Always 200 — see Task Q0/Q1: PayPal retries aggressively on anything
        // else, and the real verification/processing happens in the background
        // job this dispatched, not here.
        return Ok();
    }
}
