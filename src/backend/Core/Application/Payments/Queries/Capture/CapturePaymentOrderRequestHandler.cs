using EvrenDev.Application.Catalog.CourseEnrollments;
using EvrenDev.Application.Catalog.CourseEnrollments.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Payments;

namespace EvrenDev.Application.Payments.Queries.Capture;

public class CapturePaymentOrderRequest(Guid paymentOrderId) : IRequest<bool>
{
    public Guid PaymentOrderId { get; set; } = paymentOrderId;
}

public class CapturePaymentOrderRequestHandler(
    IRepository<PaymentOrder> paymentOrderRepository,
    IRepository<CourseEnrollment> enrollmentRepository,
    IPayPalService payPalService,
    ICurrentUser currentUser)
    : IRequestHandler<CapturePaymentOrderRequest, bool>
{
    public async Task<bool> Handle(CapturePaymentOrderRequest request, CancellationToken cancellationToken)
    {
        var paymentOrder = await paymentOrderRepository.GetByIdAsync(request.PaymentOrderId, cancellationToken)
            ?? throw new NotFoundException($"Payment order '{request.PaymentOrderId}' not found.");

        var userId = currentUser.GetUserId().ToString();
        if (paymentOrder.UserId != userId)
            throw new ForbiddenException("This payment order does not belong to the current user.");

        // Idempotent: the webhook job may have already captured this order
        // (e.g. if the buyer closed the tab right after approving, before this
        // endpoint was called) — treat re-calling capture on an already-Captured
        // order as a success rather than erroring or double-enrolling.
        if (paymentOrder.Status == PaymentOrderStatus.Captured)
            return true;

        var captureResult = await payPalService.CaptureOrderAsync(paymentOrder.PayPalOrderId, cancellationToken);

        if (!captureResult.Succeeded)
        {
            paymentOrder.Update(PaymentOrderStatus.Failed);
            await paymentOrderRepository.UpdateAsync(paymentOrder, cancellationToken);
            return false;
        }

        paymentOrder.Update(PaymentOrderStatus.Captured, captureResult.PayPalCaptureId);
        await paymentOrderRepository.UpdateAsync(paymentOrder, cancellationToken);

        var existingEnrollment = await enrollmentRepository.FirstOrDefaultAsync(
            new CourseEnrollmentByUserAndCourseSpec(paymentOrder.UserId, paymentOrder.CourseId), cancellationToken);

        // Defensive: normally impossible to already be enrolled at this point
        // (the create-order handler already checked), but the webhook job could
        // theoretically race this same capture event — never double-enroll.
        if (existingEnrollment is null)
        {
            var enrollment = CourseEnrollmentFactory.Create(paymentOrder.UserId, paymentOrder.CourseId,
                paymentOrder.Amount);
            await enrollmentRepository.AddAsync(enrollment, cancellationToken);
        }

        return true;
    }
}
