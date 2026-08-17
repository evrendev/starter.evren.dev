using EvrenDev.Domain.Catalog;

namespace EvrenDev.Domain.Payments;

public enum PaymentOrderStatus
{
    Created,
    Approved,
    Captured,
    Failed,
    Cancelled
}

// One PaymentOrder per PayPal checkout attempt for a paid course. Deliberately
// separate from CourseEnrollment (see Task Q0/Q1 design): the enrollment row is
// only created once a capture actually succeeds (via the capture endpoint or,
// as a fallback, the PAYMENT.CAPTURE.COMPLETED webhook) — a PaymentOrder can
// exist in Created/Failed/Cancelled state with no corresponding enrollment ever
// appearing, which is the whole point of gating enrollment behind payment.
public class PaymentOrder : AuditableEntity, IAggregateRoot
{
    public string UserId { get; private set; } = default!;
    public Guid CourseId { get; private set; }
    public virtual Course Course { get; private set; } = default!;

    // PayPal's own Order id (v2 Orders API) — distinct from this row's own Id.
    public string PayPalOrderId { get; private set; } = default!;
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = default!;
    public PaymentOrderStatus Status { get; private set; } = PaymentOrderStatus.Created;

    // Set once a capture actually completes.
    public string? PayPalCaptureId { get; private set; }
    public DateTime? CapturedAt { get; private set; }

    public PaymentOrder(string userId, Guid courseId, string payPalOrderId, decimal amount, string currency)
    {
        UserId = userId;
        CourseId = courseId;
        PayPalOrderId = payPalOrderId;
        Amount = amount;
        Currency = currency;
        Status = PaymentOrderStatus.Created;
    }

    public PaymentOrder Update(PaymentOrderStatus? status, string? payPalCaptureId = null)
    {
        if (status.HasValue && Status != status.Value)
            Status = status.Value;

        if (payPalCaptureId is not null && !string.Equals(PayPalCaptureId, payPalCaptureId))
            PayPalCaptureId = payPalCaptureId;

        if (Status == PaymentOrderStatus.Captured && CapturedAt is null)
            CapturedAt = DateTime.UtcNow;

        return this;
    }
}
