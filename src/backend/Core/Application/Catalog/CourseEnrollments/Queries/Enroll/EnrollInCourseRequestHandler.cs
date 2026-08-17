using EvrenDev.Application.Catalog.CourseEnrollments.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.CourseEnrollments.Queries.Enroll;

public class EnrollInCourseRequest(Guid courseId) : IRequest<bool>
{
    public Guid CourseId { get; set; } = courseId;
}

public class EnrollInCourseRequestValidator : CustomValidator<EnrollInCourseRequest>
{
    public EnrollInCourseRequestValidator(
        IReadRepository<Course> courseRepo,
        IReadRepository<CourseEnrollment> enrollmentRepo,
        ICurrentUser currentUser,
        IStringLocalizer<EnrollInCourseRequestValidator> localizer)
    {
        RuleFor(p => p.CourseId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await courseRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.courses.notfound"], id));

        RuleFor(p => p.CourseId)
            .MustAsync(async (id, ct) =>
            {
                var userId = currentUser.GetUserId().ToString();
                return await enrollmentRepo.FirstOrDefaultAsync(
                    new CourseEnrollmentByUserAndCourseSpec(userId, id), ct) is null;
            })
                .WithMessage((_, id) => string.Format(localizer["catalog.courseenrollments.create.alreadyexists"], id));
    }
}

public class EnrollInCourseRequestHandler(
    IRepository<CourseEnrollment> repository,
    IReadRepository<Course> courseRepository,
    ICurrentUser currentUser)
    : IRequestHandler<EnrollInCourseRequest, bool>
{
    public async Task<bool> Handle(EnrollInCourseRequest request, CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, cancellationToken)
            ?? throw new NotFoundException($"Course '{request.CourseId}' not found.");

        // Task Q1 payment gate: this is the free-enroll path only. Paid courses
        // (Amount > 0) must go through POST /v1/payments/orders instead, which
        // creates the CourseEnrollment itself only after a real PayPal capture
        // succeeds — see CapturePaymentOrderRequestHandler. Free courses (Amount
        // null or 0) fall through to the exact same behavior as before this task.
        if (course.Amount is > 0)
        {
            throw new ConflictException(
                $"Course '{request.CourseId}' requires payment — use the payment endpoint instead of direct enrollment.");
        }

        var existingEnrollment = await repository.FirstOrDefaultAsync(
            new CourseEnrollmentByUserAndCourseSpec(currentUser.GetUserId().ToString(), request.CourseId),
            cancellationToken);

        if (existingEnrollment is not null)
            throw new ConflictException($"You are already enrolled in course '{request.CourseId}'.");

        var enrollment = CourseEnrollmentFactory.Create(currentUser.GetUserId().ToString(), request.CourseId, 0);

        await repository.AddAsync(enrollment, cancellationToken);

        return true;
    }
}
