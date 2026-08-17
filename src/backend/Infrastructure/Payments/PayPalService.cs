using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Infrastructure.Cors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaypalServerSdk.Standard;
using PaypalServerSdk.Standard.Authentication;
using PaypalServerSdk.Standard.Models;

namespace EvrenDev.Infrastructure.Payments;

// Orders API v2 (Create/Capture) goes through the official PayPalServerSDK.
// Webhook signature verification does NOT — that controller isn't part of this
// SDK's surface (confirmed in Task Q0: doc/controllers/ only has orders,
// payments, subscriptions, transaction-search, vault), so it's a raw REST call
// to /v1/notifications/verify-webhook-signature, same "IHttpClientFactory +
// manual POST" shape as AhasendMailService.
public class PayPalService(IOptions<PayPalSettings> settings,
    IOptions<CorsSettings> corsSettings,
    IHttpClientFactory clientFactory,
    ILogger<PayPalService> logger) : IPayPalService
{
    private readonly PayPalSettings _settings = settings.Value;
    private readonly CorsSettings _corsSettings = corsSettings.Value;

    private bool IsSandbox => !string.Equals(_settings.Mode, "live", StringComparison.OrdinalIgnoreCase);

    private string ApiBaseUrl => IsSandbox
        ? "https://api-m.sandbox.paypal.com"
        : "https://api-m.paypal.com";

    private PaypalServerSdkClient BuildClient()
    {
        return new PaypalServerSdkClient.Builder()
            .ClientCredentialsAuth(
                new ClientCredentialsAuthModel.Builder(_settings.ClientId, _settings.ClientSecret).Build())
            .Environment(IsSandbox
                ? PaypalServerSdk.Standard.Environment.Sandbox
                : PaypalServerSdk.Standard.Environment.Production)
            .Build();
    }

    public async Task<PayPalCreateOrderResult> CreateOrderAsync(Guid courseId, decimal amount,
        CancellationToken cancellationToken)
    {
        var client = BuildClient();
        var currency = _settings.Currency;

        // Required for the full-page redirect flow (non-JS-SDK-in-context checkout):
        // without ReturnUrl/CancelUrl, PayPal has nowhere to send the buyer back to
        // after approval and the flow dead-ends with a generic error — confirmed the
        // hard way in Task Q1's real sandbox test. No dedicated checkout return page
        // exists in the frontend yet (that's Q2's job), so this points at an existing
        // real route as a placeholder; replace with a real return handler once one exists.
        var frontendUrl = _corsSettings.Vue ?? "https://evren.dev";

        var input = new CreateOrderInput
        {
            Body = new OrderRequest
            {
                Intent = CheckoutPaymentIntent.Capture,
                PurchaseUnits =
                [
                    new PurchaseUnitRequest
                    {
                        ReferenceId = courseId.ToString(),
                        Amount = new AmountWithBreakdown
                        {
                            CurrencyCode = currency,
                            MValue = amount.ToString("F2", CultureInfo.InvariantCulture)
                        }
                    }
                ],
                ApplicationContext = new OrderApplicationContext
                {
                    ReturnUrl = $"{frontendUrl}/learning/catalog",
                    CancelUrl = $"{frontendUrl}/learning/catalog"
                }
            }
        };

        var response = await client.OrdersController.CreateOrderAsync(input);
        var order = response.Data;

        return new PayPalCreateOrderResult
        {
            PayPalOrderId = order.Id!,
            ApproveUrl = order.Links?.FirstOrDefault(l => l.Rel == "approve")?.Href,
            Currency = currency
        };
    }

    public async Task<PayPalCaptureResult> CaptureOrderAsync(string payPalOrderId,
        CancellationToken cancellationToken)
    {
        var client = BuildClient();

        var input = new CaptureOrderInput { Id = payPalOrderId };
        var response = await client.OrdersController.CaptureOrderAsync(input);
        var order = response.Data;

        var capture = order.PurchaseUnits?.FirstOrDefault()?.Payments?.Captures?.FirstOrDefault();
        var succeeded = order.Status == OrderStatus.Completed
                        || string.Equals(capture?.Status?.ToString(), "COMPLETED", StringComparison.OrdinalIgnoreCase);

        return new PayPalCaptureResult
        {
            Succeeded = succeeded,
            PayPalCaptureId = capture?.Id
        };
    }

    public async Task<bool> VerifyWebhookSignatureAsync(IReadOnlyDictionary<string, string> headers, string rawBody,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_settings.WebhookId))
        {
            logger.LogWarning("PayPal WebhookId is not configured — cannot verify webhook signature.");
            return false;
        }

        try
        {
            var httpClient = clientFactory.CreateClient();

            var accessToken = await GetAccessTokenAsync(httpClient, cancellationToken);

            var webhookEvent = JsonSerializer.Deserialize<JsonElement>(rawBody);

            var verifyBody = new
            {
                auth_algo = GetHeader(headers, "paypal-auth-algo"),
                cert_url = GetHeader(headers, "paypal-cert-url"),
                transmission_id = GetHeader(headers, "paypal-transmission-id"),
                transmission_sig = GetHeader(headers, "paypal-transmission-sig"),
                transmission_time = GetHeader(headers, "paypal-transmission-time"),
                webhook_id = _settings.WebhookId,
                webhook_event = webhookEvent
            };

            var request = new HttpRequestMessage(HttpMethod.Post,
                $"{ApiBaseUrl}/v1/notifications/verify-webhook-signature")
            {
                Content = new StringContent(JsonSerializer.Serialize(verifyBody), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.SendAsync(request, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("PayPal verify-webhook-signature call failed with {StatusCode}: {Body}",
                    response.StatusCode, responseBody);
                return false;
            }

            using var doc = JsonDocument.Parse(responseBody);
            var status = doc.RootElement.TryGetProperty("verification_status", out var statusProp)
                ? statusProp.GetString()
                : null;

            return string.Equals(status, "SUCCESS", StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to verify PayPal webhook signature.");
            return false;
        }
    }

    private async Task<string> GetAccessTokenAsync(HttpClient httpClient, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{ApiBaseUrl}/v1/oauth2/token")
        {
            Content = new FormUrlEncodedContent([new KeyValuePair<string, string>("grant_type", "client_credentials")])
        };

        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("access_token").GetString()!;
    }

    private static string GetHeader(IReadOnlyDictionary<string, string> headers, string name)
    {
        return headers.TryGetValue(name, out var value) ? value : string.Empty;
    }
}
