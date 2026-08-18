using EvrenDev.Application.Catalog.CourseEnrollments.Specifications;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.CourseEnrollments.Queries.AdminEnroll;

// Admin-only counterpart to EnrollInCourseRequest (Queries/Enroll) — takes an
// explicit userId instead of the calling user, and deliberately bypasses the
// Task Q1 Amount > 0 payment gate: an admin adding a student manually isn't
// taking a payment, so PricePaid is always 0 regardless of the course's price.
public class AdminEnrollStudentRequest(string userId, Guid courseId) : IRequest<bool>
{
    public string UserId { get; set; } = userId;
    public Guid CourseId { get; set; } = courseId;
}

public class AdminEnrollStudentRequestValidator : CustomValidator<AdminEnrollStudentRequest>
{
    public AdminEnrollStudentRequestValidator(
        IReadRepository<Course> courseRepo,
        IReadRepository<CourseEnrollment> enrollmentRepo,
        IStringLocalizer<AdminEnrollStudentRequestValidator> localizer)
    {
        RuleFor(p => p.UserId).NotEmpty();

        RuleFor(p => p.CourseId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await courseRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => string.Format(localizer["catalog.courses.notfound"], id));

        RuleFor(p => p)
            .MustAsync(async (p, ct) => await enrollmentRepo.FirstOrDefaultAsync(
                new CourseEnrollmentByUserAndCourseSpec(p.UserId, p.CourseId), ct) is null)
                .WithMessage((_, p) => string.Format(localizer["catalog.courseenrollments.create.alreadyexists"], p.CourseId));
    }
}

public class AdminEnrollStudentRequestHandler(IRepository<CourseEnrollment> repository)
    : IRequestHandler<AdminEnrollStudentRequest, bool>
{
    public async Task<bool> Handle(AdminEnrollStudentRequest request, CancellationToken cancellationToken)
    {
        var enrollment = CourseEnrollmentFactory.Create(request.UserId, request.CourseId, 0);

        await repository.AddAsync(enrollment, cancellationToken);

        return true;
    }
}
