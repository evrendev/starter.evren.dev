using System.Text.Json;
using EvrenDev.Application.Catalog.CourseEnrollments;
using EvrenDev.Application.Catalog.CourseEnrollments.Specifications;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Application.Payments.Interfaces;
using EvrenDev.Application.Payments.Specifications;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Payments;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Infrastructure.BackgroundJobs;

public class ProcessPayPalWebhookJob(
    IPayPalService payPalService,
    IRepository<PaymentOrder> paymentOrderRepository,
    IRepository<CourseEnrollment> enrollmentRepository,
    ILogger<ProcessPayPalWebhookJob> logger) : IProcessPayPalWebhookJob
{
    private const string OrderApproved = "CHECKOUT.ORDER.APPROVED";
    private const string CaptureCompleted = "PAYMENT.CAPTURE.COMPLETED";

    public async Task ExecuteAsync(string rawBody, Dictionary<string, string> headers,
        CancellationToken cancellationToken)
    {
        var verified = await payPalService.VerifyWebhookSignatureAsync(headers, rawBody, cancellationToken);
        if (!verified)
        {
            logger.LogWarning("PayPal webhook signature verification failed — dropping event.");
            return;
        }

        using var doc = JsonDocument.Parse(rawBody);
        var root = doc.RootElement;

        var eventType = root.TryGetProperty("event_type", out var eventTypeProp) ? eventTypeProp.GetString() : null;
        if (eventType is not (OrderApproved or CaptureCompleted))
        {
            logger.LogInformation("Ignoring PayPal webhook event type '{EventType}' — not handled.", eventType);
            return;
        }

        if (!root.TryGetProperty("resource", out var resource))
            return;

        var payPalOrderId = ExtractOrderId(resource, eventType);
        if (string.IsNullOrEmpty(payPalOrderId))
        {
            logger.LogWarning("Could not extract PayPal order id from '{EventType}' webhook event.", eventType);
            return;
        }

        var paymentOrder = await paymentOrderRepository.FirstOrDefaultAsync(
            new PaymentOrderByPayPalOrderIdSpec(payPalOrderId), cancellationToken);

        if (paymentOrder is null)
        {
            logger.LogWarning("No PaymentOrder found for PayPal order '{PayPalOrderId}'.", payPalOrderId);
            return;
        }

        // Idempotent guard: Hangfire may retry this job (transient DB errors etc.)
        // and PayPal may also send the same event more than once (their own retry
        // policy on their side) — once captured, further deliveries are no-ops.
        if (paymentOrder.Status == PaymentOrderStatus.Captured)
            return;

        if (eventType == OrderApproved)
        {
            paymentOrder.Update(PaymentOrderStatus.Approved);
            await paymentOrderRepository.UpdateAsync(paymentOrder, cancellationToken);
            return;
        }

        // CaptureCompleted: the capture id is the resource's own id here (the
        // resource IS the Capture object), the order id was under
        // supplementary_data.related_ids.order_id (see ExtractOrderId).
        var captureId = resource.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;

        paymentOrder.Update(PaymentOrderStatus.Captured, captureId);
        await paymentOrderRepository.UpdateAsync(paymentOrder, cancellationToken);

        var existingEnrollment = await enrollmentRepository.FirstOrDefaultAsync(
            new CourseEnrollmentByUserAndCourseSpec(paymentOrder.UserId, paymentOrder.CourseId), cancellationToken);

        // Fallback path: normally CapturePaymentOrderRequestHandler already created
        // the enrollment synchronously when the frontend called the capture endpoint.
        // This webhook is the safety net for the case where that call never happened
        // (browser closed, network drop right after approval) — never double-enroll.
        if (existingEnrollment is null)
        {
            var enrollment = CourseEnrollmentFactory.Create(paymentOrder.UserId, paymentOrder.CourseId,
                paymentOrder.Amount);
            await enrollmentRepository.AddAsync(enrollment, cancellationToken);
        }
    }

    private static string? ExtractOrderId(JsonElement resource, string? eventType)
    {
        if (eventType == OrderApproved)
            return resource.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;

        // PAYMENT.CAPTURE.COMPLETED: resource is the Capture, the parent Order id
        // lives under supplementary_data.related_ids.order_id.
        return resource.TryGetProperty("supplementary_data", out var supplementaryData)
               && supplementaryData.TryGetProperty("related_ids", out var relatedIds)
               && relatedIds.TryGetProperty("order_id", out var orderIdProp)
            ? orderIdProp.GetString()
            : null;
    }
}
