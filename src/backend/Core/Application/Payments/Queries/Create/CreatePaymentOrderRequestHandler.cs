using EvrenDev.Application.Catalog.CourseEnrollments.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;
using EvrenDev.Domain.Payments;

namespace EvrenDev.Application.Payments.Queries.Create;

public class CreatePaymentOrderRequest(Guid courseId) : IRequest<CreatePaymentOrderResponse>
{
    public Guid CourseId { get; set; } = courseId;
}

public class CreatePaymentOrderResponse
{
    public Guid PaymentOrderId { get; set; }
    public string PayPalOrderId { get; set; } = default!;
    public string? ApproveUrl { get; set; }
}

public class CreatePaymentOrderRequestValidator : CustomValidator<CreatePaymentOrderRequest>
{
    public CreatePaymentOrderRequestValidator(IReadRepository<Course> courseRepo,
        IStringLocalizer<CreatePaymentOrderRequestValidator> localizer)
    {
        RuleFor(p => p.CourseId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await courseRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.courses.notfound"], id));
    }
}

// NOTE (see EnrollInCourseRequestHandler / ImportPagesFromPptxRequestHandler for
// the same caveat): FluentValidation validators are never actually invoked by any
// MediatR pipeline in this codebase, so the checks below are re-asserted directly
// in the handler — the validator above exists for documentation/consistency only.
public class CreatePaymentOrderRequestHandler(
    IReadRepository<Course> courseRepository,
    IRepository<PaymentOrder> paymentOrderRepository,
    IReadRepository<CourseEnrollment> enrollmentRepository,
    IPayPalService payPalService,
    ICurrentUser currentUser)
    : IRequestHandler<CreatePaymentOrderRequest, CreatePaymentOrderResponse>
{
    public async Task<CreatePaymentOrderResponse> Handle(CreatePaymentOrderRequest request,
        CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, cancellationToken)
            ?? throw new NotFoundException($"Course '{request.CourseId}' not found.");

        // This endpoint is the paid-course counterpart to EnrollInCourseRequest —
        // free courses must keep using that one (unchanged), not this.
        if (course.Amount is not > 0)
            throw new ConflictException($"Course '{request.CourseId}' is free — use the standard enroll endpoint.");

        var userId = currentUser.GetUserId().ToString();

        var existingEnrollment = await enrollmentRepository.FirstOrDefaultAsync(
            new CourseEnrollmentByUserAndCourseSpec(userId, request.CourseId), cancellationToken);

        if (existingEnrollment is not null)
            throw new ConflictException($"You are already enrolled in course '{request.CourseId}'.");

        var createResult = await payPalService.CreateOrderAsync(request.CourseId, course.Amount!.Value,
            cancellationToken);

        var paymentOrder = new PaymentOrder(userId, request.CourseId, createResult.PayPalOrderId,
            course.Amount!.Value, createResult.Currency);

        await paymentOrderRepository.AddAsync(paymentOrder, cancellationToken);

        return new CreatePaymentOrderResponse
        {
            PaymentOrderId = paymentOrder.Id,
            PayPalOrderId = createResult.PayPalOrderId,
            ApproveUrl = createResult.ApproveUrl
        };
    }
}
