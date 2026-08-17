using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Payments.Interfaces;

namespace EvrenDev.Application.Payments.Queries.Webhook;

// Called synchronously from the webhook controller action — its ONLY job is to
// enqueue the real work and return immediately, so the controller can answer
// PayPal with 200 fast. PayPal retries aggressively (hours) on slow/failed
// responses, and the real work here (an outbound signature-verification call
// to PayPal, then DB writes) is exactly the kind of latency that shouldn't sit
// in the request path — see Task Q0/Q1 design. Same "enqueue a job interface"
// shape as ImportPagesFromPptxRequestHandler, not a raw MediatR-in-Hangfire call.
public class ProcessPayPalWebhookRequest(string rawBody, Dictionary<string, string> headers) : IRequest
{
    public string RawBody { get; set; } = rawBody;
    public Dictionary<string, string> Headers { get; set; } = headers;
}

public class ProcessPayPalWebhookRequestHandler(IJobService jobService)
    : IRequestHandler<ProcessPayPalWebhookRequest>
{
    public Task Handle(ProcessPayPalWebhookRequest request, CancellationToken cancellationToken)
    {
        jobService.Enqueue<IProcessPayPalWebhookJob>(
            x => x.ExecuteAsync(request.RawBody, request.Headers, default));

        return Task.CompletedTask;
    }
}
