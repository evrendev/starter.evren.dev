using EvrenDev.Application.Catalog.CourseEnrollments.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.CourseEnrollments.Queries.RemoveEnrollment;

// Admin-only unenroll — no self-service equivalent exists (or is planned);
// CourseEnrollment's composite key (UserId, CourseId) is looked up directly,
// there's no surrogate Id to key this request by.
public class RemoveEnrollmentRequest(string userId, Guid courseId) : IRequest<bool>
{
    public string UserId { get; set; } = userId;
    public Guid CourseId { get; set; } = courseId;
}

public class RemoveEnrollmentRequestHandler(IRepository<CourseEnrollment> repository)
    : IRequestHandler<RemoveEnrollmentRequest, bool>
{
    public async Task<bool> Handle(RemoveEnrollmentRequest request, CancellationToken cancellationToken)
    {
        var enrollment = await repository.FirstOrDefaultAsync(
            new CourseEnrollmentByUserAndCourseSpec(request.UserId, request.CourseId),
            cancellationToken);

        _ = enrollment ?? throw new NotFoundException(
            $"Enrollment for user '{request.UserId}' in course '{request.CourseId}' not found.");

        await repository.DeleteAsync(enrollment, cancellationToken);

        return true;
    }
}
